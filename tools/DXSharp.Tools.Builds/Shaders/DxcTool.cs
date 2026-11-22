using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DXSharp.Tools.Builds.Shaders;

/// <summary>Compiles every supplied shader using a pinned DXC executable and content-based incremental state.</summary>
public sealed class DxcTool : Microsoft.Build.Utilities.Task {
    /// <summary>HLSL sources with Profile, EntryPoint, OutputFile, Defines and IncludeDirectories metadata.</summary>
    [Required] public ITaskItem[] ShaderFiles { get; set; } = Array.Empty<ITaskItem>();
    /// <summary>Absolute path to the selected compiler executable.</summary>
    [Required] public string CompilerPath { get; set; } = string.Empty;
    /// <summary>Project-relative paths are resolved against this directory.</summary>
    [Required] public string ProjectDirectory { get; set; } = string.Empty;
    /// <summary>Emit shader PDBs and disable optimization.</summary>
    public bool DebugShaders { get; set; }
    /// <summary>Promote compiler warnings to build errors.</summary>
    public bool WarningsAsErrors { get; set; }
    /// <summary>Successfully compiled or up-to-date output containers.</summary>
    [Output] public ITaskItem[] Outputs { get; private set; } = Array.Empty<ITaskItem>();
    private static readonly Regex Lines = new Regex("^#(?:line)?\\s+\\d+\\s+\"([^\"]+)\"", RegexOptions.Multiline);

    /// <inheritdoc />
    public override bool Execute() {
        var outputs = new List<ITaskItem>();
        try {
            CompilerPath = Path.GetFullPath(CompilerPath);
            if (!File.Exists(CompilerPath)) throw new FileNotFoundException("The pinned DXC executable was not found.", CompilerPath);
            var compiler = new StringBuilder(HashFile(CompilerPath));
            foreach (var name in new[] { "dxcompiler.dll", "dxil.dll", "../lib/libdxcompiler.so", "../lib/libdxil.so" }) {
                string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(CompilerPath)!, name));
                if (File.Exists(path)) compiler.Append(HashFile(path));
            }
            var occupied = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in ShaderFiles) {
                string path = Absolute(item.GetMetadata("OutputFile"));
                if (!occupied.Add(path)) throw new InvalidOperationException("Multiple shader items have the same output: " + path);
            }
            foreach (var item in ShaderFiles) {
                try { outputs.Add(Compile(item, compiler.ToString())); }
                catch (Exception error) { Log.LogErrorFromException(error, false); }
            }
        } catch (Exception error) { Log.LogErrorFromException(error, false); }
        Outputs = outputs.ToArray(); return !Log.HasLoggedErrors;
    }
    private ITaskItem Compile(ITaskItem item, string compiler) {
        string source = Absolute(item.ItemSpec), output = Absolute(item.GetMetadata("OutputFile"));
        string profile = item.GetMetadata("Profile"), entry = item.GetMetadata("EntryPoint");
        if (string.IsNullOrWhiteSpace(profile)) throw new ArgumentException("Shader Profile is required: " + source);
        if (!profile.StartsWith("lib_", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(entry)) throw new ArgumentException("Shader EntryPoint is required: " + source);
        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        var args = new List<string> { source, "-T", profile, "-HV", "2021" };
        if (!string.IsNullOrWhiteSpace(entry)) { args.Add("-E"); args.Add(entry); }
        foreach (var value in Split(item.GetMetadata("Defines"))) { args.Add("-D"); args.Add(value); }
        foreach (var value in Split(item.GetMetadata("IncludeDirectories"))) { args.Add("-I"); args.Add(Absolute(value)); }
        args.AddRange(Split(item.GetMetadata("AdditionalArguments")));
        string temporary = output + ".tmp-" + Guid.NewGuid().ToString("N");
        string preprocess = temporary + ".hlsl", state = output + ".state", pdb = output + ".pdb", manifest = output + ".xml";
        try {
            // DXC performs include/macro resolution on every build. Hashing its preprocessed result also
            // catches macro includes, transitive edits, deletion and include-path shadowing, including spaces.
            Invoke(args.Concat(new[] { "-P", "-Fi", preprocess }));
            string text = File.ReadAllText(preprocess);
            string fingerprint = Hash(compiler + "\n" + string.Join("\n", args) + "\n" + DebugShaders + WarningsAsErrors + "\n" + text);
            string existing = File.Exists(state) ? File.ReadAllText(state) : string.Empty;
            bool current = File.Exists(output) && File.Exists(manifest) && (!DebugShaders || File.Exists(pdb)) &&
                existing == fingerprint + "\n" + HashFile(output);
            if (!current) {
                var compile = new List<string>(args) { "-Fo", temporary };
                if (DebugShaders) compile.AddRange(new[] { "-Zi", "-Od", "-Fd", pdb }); else compile.Add("-O3");
                if (WarningsAsErrors) compile.Add("-WX");
                Invoke(compile);
                if (!File.Exists(temporary) || new FileInfo(temporary).Length < 32) throw new InvalidDataException("DXC produced no shader container: " + source);
                // Invalidate incremental state before replacing output. A failed build never becomes up-to-date.
                if (File.Exists(state)) File.Delete(state);
                File.Copy(temporary, output, true);
                var dependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { source };
                foreach (Match match in Lines.Matches(text)) {
                    string path = match.Groups[1].Value.Replace("\\\\", "\\");
                    if (path.StartsWith("<", StringComparison.Ordinal)) continue;
                    path = Absolute(path); if (File.Exists(path)) dependencies.Add(path);
                }
                new XDocument(new XElement("Shader", new XAttribute("Source", source), new XAttribute("Profile", profile),
                    new XAttribute("EntryPoint", entry), new XAttribute("CompilerFingerprint", compiler), new XAttribute("Fingerprint", fingerprint),
                    dependencies.OrderBy(x => x, StringComparer.Ordinal).Select(x => new XElement("Dependency", new XAttribute("Path", x), new XAttribute("Sha256", HashFile(x)))))).Save(manifest);
                File.WriteAllText(state, fingerprint + "\n" + HashFile(output));
                if (!DebugShaders && File.Exists(pdb)) File.Delete(pdb);
                Log.LogMessage(MessageImportance.High, "DXC compiled: {0} ({1}, {2})", source, entry, profile);
            } else Log.LogMessage(MessageImportance.High, "DXC up-to-date: {0} ({1}, {2})", source, entry, profile);
            var result = new TaskItem(output); result.SetMetadata("LogicalName", item.GetMetadata("LogicalName"));
            result.SetMetadata("TargetPath", item.GetMetadata("TargetPath")); return result;
        } finally {
            if (File.Exists(temporary)) File.Delete(temporary);
            if (File.Exists(preprocess)) File.Delete(preprocess);
        }
    }
    private void Invoke(IEnumerable<string> args) {
        string command = string.Join(" ", args.Select(Quote));
        using (var process = new Process()) {
            process.StartInfo = new ProcessStartInfo(CompilerPath, command.ToString()) { UseShellExecute = false,
                RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true, WorkingDirectory = ProjectDirectory };
            process.Start(); var stdout = process.StandardOutput.ReadToEndAsync(); var stderr = process.StandardError.ReadToEndAsync();
            if (!process.WaitForExit(120000)) { process.Kill(); process.WaitForExit(); throw new TimeoutException("DXC exceeded two minutes."); }
            string text = stdout.GetAwaiter().GetResult() + stderr.GetAwaiter().GetResult();
            foreach (string line in text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                var match = Regex.Match(line, @"^(.*?):(\d+):(\d+):\s*(warning|error):\s*(.*)$");
                if (match.Success) {
                    int row = int.Parse(match.Groups[2].Value), col = int.Parse(match.Groups[3].Value);
                    if (match.Groups[4].Value == "error" || WarningsAsErrors) Log.LogError(null, "DXSHADER", null, match.Groups[1].Value, row, col, 0, 0, match.Groups[5].Value);
                    else Log.LogWarning(null, "DXSHADER", null, match.Groups[1].Value, row, col, 0, 0, match.Groups[5].Value);
                } else Log.LogMessage(MessageImportance.Normal, line);
            }
            if (process.ExitCode != 0) throw new InvalidOperationException("DXC failed with exit code " + process.ExitCode + ": " + text);
        }
    }
    private static string Quote(string value) {
        if (value.Length != 0 && !value.Any(c => char.IsWhiteSpace(c) || c == '"')) return value;
        var result = new StringBuilder("\""); int slashes = 0;
        foreach (char c in value) {
            if (c == '\\') { ++slashes; continue; }
            result.Append('\\', c == '"' ? slashes * 2 + 1 : slashes); result.Append(c); slashes = 0;
        }
        result.Append('\\', slashes * 2); return result.Append('"').ToString();
    }
    private string Absolute(string path) {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("A shader path is empty.");
        return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(ProjectDirectory, path));
    }
    private static IEnumerable<string> Split(string value) => value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.Length != 0);
    private static string Hash(string value) { using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(value))); }
    private static string HashFile(string path) { using (var sha = SHA256.Create()) using (var stream = File.OpenRead(path)) return Convert.ToBase64String(sha.ComputeHash(stream)); }
}
