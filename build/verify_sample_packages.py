"""Build every runnable sample against freshly packed SDK packages outside the checkout."""

import argparse
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import xml.etree.ElementTree as ET

SDK_PACKAGES = ("DXSharp.Native", "DXSharp.Core", "DXSharp.DirectML", "DXSharp.Framework.Desktop", "DXSharp.Assets")
EXCLUDED = {"bin", "obj", "artifacts", ".vs"}


def run(arguments, directory, log):
    with log.open("w", encoding="utf-8") as output:
        process = subprocess.Popen(["dotnet", *arguments], cwd=directory, stdout=subprocess.PIPE,
                                   stderr=subprocess.STDOUT, text=True, encoding="utf-8", errors="replace")
        for line in process.stdout:
            print(line, end="", flush=True)
            output.write(line)
        if process.wait():
            raise RuntimeError(f"dotnet {' '.join(arguments)} failed; inspect {log}")


def copy_source(source, destination):
    shutil.copytree(source, destination, dirs_exist_ok=True,
                    ignore=lambda path, names: [name for name in names if name in EXCLUDED or name == "packages.lock.json"])


def qualify(configuration):
    root = Path(__file__).resolve().parent.parent
    packages = root / "artifacts/packages" / configuration
    evidence = root / "artifacts/sample-packages" / configuration
    evidence.mkdir(parents=True, exist_ok=True)
    version = subprocess.check_output(["dotnet", "msbuild", str(root / "src/DXSharp.Core/DXSharp.Core.csproj"),
                                       "-getProperty:PackageVersion", "-nologo"], text=True).strip()
    for package in SDK_PACKAGES:
        if not (packages / f"{package}.{version}.nupkg").is_file():
            raise FileNotFoundError(f"Missing SDK package: {package} {version}")
    source_projects = sorted(project for project in (root / "samples").rglob("*.csproj")
                             if not set(project.relative_to(root).parts) & EXCLUDED)
    if not source_projects:
        raise RuntimeError("No sample projects found")
    solution_text = (root / "DXSharp.sln").read_text(encoding="utf-8-sig").replace("\\", "/").casefold()
    manifest = []
    with tempfile.TemporaryDirectory(prefix="dxsharp-samples-") as temporary:
        consumer = Path(temporary)
        for source in source_projects:
            relative = source.relative_to(root)
            if relative.as_posix().casefold() not in solution_text:
                raise RuntimeError(f"Sample absent from DXSharp.sln: {relative}")
            target = consumer / relative
            copy_source(source.parent, target.parent)
            project = ET.parse(target)
            dependencies = []
            for group in project.getroot().findall("ItemGroup"):
                for reference in list(group.findall("ProjectReference")):
                    package = Path(reference.attrib["Include"].replace("\\", "/")).stem
                    if package not in SDK_PACKAGES:
                        raise RuntimeError(f"Unexpected sample dependency {package}: {relative}")
                    group.remove(reference)
                    ET.SubElement(group, "PackageReference", Include=package)
                    dependencies.append(package)
                for reference in group.findall("PackageReference"):
                    if reference.attrib.get("Include", "").startswith("Microsoft.Windows.CsWin32"):
                        raise RuntimeError(f"Sample regenerates the shared native bindings: {relative}")
            if not dependencies:
                raise RuntimeError(f"Sample has no current SDK dependency: {relative}")
            if project.getroot().findall(".//ProjectReference"):
                raise RuntimeError(f"Unconverted conditional project reference in {relative}")
            project.write(target, encoding="utf-8", xml_declaration=True)
            manifest.append({"project": relative.as_posix(), "packages": dependencies, "version": version})
        copy_source(root / "samples/Shared", consumer / "samples/Shared")
        shutil.copy2(root / "samples/Directory.Build.props", consumer / "samples/Directory.Build.props")
        shutil.copy2(root / "global.json", consumer / "global.json")
        (consumer / "Directory.Build.props").write_text("""<Project><PropertyGroup>
  <DotNetTargets>net10.0-windows10.0.26100.0</DotNetTargets><TargetCPUs>AnyCPU;x64</TargetCPUs>
  <EnableWindowsTargeting>true</EnableWindowsTargeting><AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  <ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><LangVersion>14.0</LangVersion>
</PropertyGroup></Project>""", encoding="utf-8")
        versions = ET.parse(root / "Directory.Packages.props")
        group = ET.SubElement(versions.getroot(), "ItemGroup")
        for package in SDK_PACKAGES:
            ET.SubElement(group, "PackageVersion", Include=package, Version=f"[{version}]")
        versions.write(consumer / "Directory.Packages.props", encoding="utf-8", xml_declaration=True)
        solution = ET.Element("Solution")
        for entry in manifest:
            ET.SubElement(solution, "Project", Path=entry["project"])
        ET.ElementTree(solution).write(consumer / "Samples.slnx", encoding="utf-8", xml_declaration=True)
        config = ET.Element("configuration")
        sources = ET.SubElement(config, "packageSources")
        ET.SubElement(sources, "clear")
        ET.SubElement(sources, "add", key="local", value=str(packages))
        ET.SubElement(sources, "add", key="nuget.org", value="https://api.nuget.org/v3/index.json")
        mapping = ET.SubElement(config, "packageSourceMapping")
        for key, pattern in (("local", "DXSharp.*"), ("nuget.org", "*")):
            ET.SubElement(ET.SubElement(mapping, "packageSource", key=key), "package", pattern=pattern)
        ET.ElementTree(config).write(consumer / "NuGet.Config", encoding="utf-8", xml_declaration=True)
        run(["restore", "Samples.slnx", "--packages", str(consumer / "packages"), "-m:1", "-p:Platform=Any CPU"], consumer, evidence / "restore.log")
        run(["build", "Samples.slnx", "-c", configuration, "--no-restore", "-m:1", "-p:Platform=Any CPU"], consumer, evidence / "build.log")
        if os.name == "nt":
            # The developer shell's Platform variable must not change the built output path.
            fleet = consumer / "samples/FleetBattler/bin" / configuration / "net10.0-windows10.0.26100.0/FleetBattler.dll"
            run([str(fleet), "--warp", "--mute", "--frames=14", "--view-test"], consumer, evidence / "fleet-runtime.log")
        (evidence / "samples.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
        print(f"PASS: all {len(manifest)} runnable samples consume packaged SDK {version} outside the checkout.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--configuration", choices=("Debug", "Release"), default="Debug")
    qualify(parser.parse_args().configuration)
