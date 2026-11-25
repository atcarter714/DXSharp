"""Build every runnable sample against freshly packed SDK packages outside the checkout."""

import argparse
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import time
import urllib.error
import urllib.request
import xml.etree.ElementTree as ET

SDK_PACKAGES = ("DXSharp.Native", "DXSharp.Core", "DXSharp.DirectML", "DXSharp.Framework.Desktop", "DXSharp.Framework.Wpf", "DXSharp.Framework.WinUI", "DXSharp.Assets", "DXSharp.Framework", "DXSharp.Media")
PUBLIC_PACKAGES = (*SDK_PACKAGES, "DXSharp.Tools.Builds")
EXCLUDED = {"bin", "obj", "artifacts", ".vs"}


def run(arguments, directory, log, executable=False):
    # Isolated package caches must not remain loaded in reusable MSBuild nodes.
    environment = {**os.environ, "MSBUILDDISABLENODEREUSE": "1", "DOTNET_CLI_USE_MSBUILD_SERVER": "0"}
    with log.open("w", encoding="utf-8") as output:
        process = subprocess.Popen(arguments if executable else ["dotnet", *arguments], cwd=directory, stdout=subprocess.PIPE,
                                   stderr=subprocess.STDOUT, text=True, encoding="utf-8", errors="replace", env=environment)
        for line in process.stdout:
            print(line, end="", flush=True)
            output.write(line)
        if process.wait():
            raise RuntimeError(f"dotnet {' '.join(arguments)} failed; inspect {log}")


def copy_source(source, destination):
    shutil.copytree(source, destination, dirs_exist_ok=True,
                    ignore=lambda path, names: [name for name in names if name in EXCLUDED or (name.startswith("packages.") and name.endswith(".lock.json"))])


def wait_for_public_feed(version, timeout=600):
    deadline = time.monotonic() + timeout
    pending = set(PUBLIC_PACKAGES)
    while pending:
        for package in sorted(pending):
            remaining = deadline - time.monotonic()
            if remaining <= 0:
                raise TimeoutError(f"NuGet.org indexing timed out for {version}: {sorted(pending)}")
            try:
                url = f"https://api.nuget.org/v3-flatcontainer/{package.lower()}/index.json"
                with urllib.request.urlopen(url, timeout=min(15, remaining)) as response:
                    versions = json.load(response)["versions"]
                if version in versions:
                    pending.remove(package)
            except (urllib.error.URLError, TimeoutError, OSError):
                pass  # Feed visibility/network failures retry only within the bounded indexing window.
        if pending:
            print(f"Waiting for NuGet.org {version}: {', '.join(sorted(pending))}", flush=True)
            time.sleep(min(15, max(0, deadline - time.monotonic())))


def qualify(configuration, runtime_major, version=None, public_feed=False):
    root = Path(__file__).resolve().parent.parent
    framework = f"net{runtime_major}.0-windows10.0.26100.0"
    packages = root / "artifacts/packages" / configuration
    evidence = root / ("artifacts/public-packages" if public_feed else "artifacts/sample-packages") / configuration / f"net{runtime_major}"
    evidence.mkdir(parents=True, exist_ok=True)
    version = version or subprocess.check_output(["dotnet", "msbuild", str(root / "src/DXSharp.Core/DXSharp.Core.csproj"),
                                       "-getProperty:PackageVersion", "-nologo"], text=True).strip()
    if public_feed:
        wait_for_public_feed(version)
    else:
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
            manifest.append({"project": relative.as_posix(), "packages": dependencies, "version": version, "framework": framework})
        copy_source(root / "samples/Shared", consumer / "samples/Shared")
        shutil.copy2(root / "samples/Directory.Build.props", consumer / "samples/Directory.Build.props")
        shutil.copy2(root / "global.json", consumer / "global.json")
        (consumer / "Directory.Build.props").write_text("""<Project><PropertyGroup>
  <DotNetTargets>net10.0-windows10.0.26100.0</DotNetTargets><DXSharpTargetFrameworks>net10.0-windows10.0.26100.0</DXSharpTargetFrameworks><TargetCPUs>AnyCPU;x64</TargetCPUs>
  <EnableWindowsTargeting>true</EnableWindowsTargeting><AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  <ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><LangVersion>14.0</LangVersion>
</PropertyGroup></Project>""".replace("net10.0-windows10.0.26100.0", framework), encoding="utf-8")
        versions = ET.parse(root / "Directory.Packages.props")
        group = ET.SubElement(versions.getroot(), "ItemGroup")
        for package in PUBLIC_PACKAGES:
            ET.SubElement(group, "PackageVersion", Include=package, Version=f"[{version}]")
        versions.write(consumer / "Directory.Packages.props", encoding="utf-8", xml_declaration=True)
        solution = ET.Element("Solution")
        for entry in manifest:
            ET.SubElement(solution, "Project", Path=entry["project"])
        if public_feed:
            shader = consumer / "tools/ShaderConsumer"
            shader.mkdir(parents=True)
            (shader / "ShaderConsumer.csproj").write_text(f"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>{framework}</TargetFramework></PropertyGroup>
  <ItemGroup>
    <PackageReference Include="DXSharp.Tools.Builds" PrivateAssets="all" />
    <DXSharpShader Include="Smoke.hlsl" EntryPoint="CS" Profile="cs_6_0" LogicalName="PublicFeed.Smoke.CS.dxil" />
  </ItemGroup>
</Project>""", encoding="utf-8")
            (shader / "Smoke.hlsl").write_text("[numthreads(1,1,1)] void CS() {}\n", encoding="utf-8")
            ET.SubElement(solution, "Project", Path="tools/ShaderConsumer/ShaderConsumer.csproj")
        ET.ElementTree(solution).write(consumer / "Samples.slnx", encoding="utf-8", xml_declaration=True)
        config = ET.Element("configuration")
        sources = ET.SubElement(config, "packageSources")
        ET.SubElement(sources, "clear")
        if not public_feed:
            ET.SubElement(sources, "add", key="local", value=str(packages))
        ET.SubElement(sources, "add", key="nuget.org", value="https://api.nuget.org/v3/index.json")
        mapping = ET.SubElement(config, "packageSourceMapping")
        rules = (("nuget.org", "*"),) if public_feed else (("local", "DXSharp.*"), ("nuget.org", "*"))
        for key, pattern in rules:
            ET.SubElement(ET.SubElement(mapping, "packageSource", key=key), "package", pattern=pattern)
        ET.ElementTree(config).write(consumer / "NuGet.Config", encoding="utf-8", xml_declaration=True)
        run(["restore", "Samples.slnx", "--packages", str(consumer / "packages"), "-m:1", "-nr:false", "-p:Platform=Any CPU"], consumer, evidence / "restore.log")
        run(["build", "Samples.slnx", "-c", configuration, "--no-restore", "-m:1", "-nr:false", "-p:UseSharedCompilation=false", "-p:Platform=Any CPU"], consumer, evidence / "build.log")
        if os.name == "nt":
            # The developer shell's Platform variable must not change the built output path.
            fleet = consumer / "samples/FleetBattler/bin" / configuration / framework / "FleetBattler.dll"
            run([str(fleet), "--warp", "--mute", "--frames=14", "--view-test"], consumer, evidence / "fleet-runtime.log")
            character = consumer / "samples/CharacterLab/bin" / configuration / framework / "CharacterLab.dll"
            run([str(character), "--warp", "--frames=9", "--close-test"], consumer, evidence / "character-runtime.log")
            native = consumer / "samples/CharacterLab.Native/bin" / configuration / framework / "CharacterLab.Native.dll"
            runtime = json.loads(native.with_suffix('.runtimeconfig.json').read_text())['runtimeOptions']
            runtime_frameworks = runtime.get('frameworks', [runtime.get('framework', {})])
            if any(x.get('name', '').startswith('Microsoft.WindowsDesktop') for x in runtime_frameworks):
                raise RuntimeError('Native sample requires the Windows Desktop runtime')
            for name in ('dxcompiler.dll', 'dxil.dll'):
                path = native.parent / name
                if path.exists():
                    raise RuntimeError(f'Native precompiled consumer deployed {name}')
            run([str(native), "--warp", "--frames=9", "--resize-test"], consumer, evidence / "character-native-runtime.log")
            wpf = consumer / "samples/CharacterLab.Wpf/bin" / configuration / framework / "CharacterLab.Wpf.dll"
            for name in ('dxcompiler.dll', 'dxil.dll'):
                if (wpf.parent / name).exists():
                    raise RuntimeError(f'WPF precompiled consumer deployed {name}')
            run([str(wpf), "--warp", "--frames=9"], consumer, evidence / "character-wpf-runtime.log")
            run([str(wpf), "--warp", "--host-test"], consumer, evidence / "character-wpf-host.log")
            winui_project = consumer / 'samples/CharacterLab.WinUI/CharacterLab.WinUI.csproj'
            winui = Path(subprocess.check_output(['dotnet', 'msbuild', str(winui_project), f'-p:Configuration={configuration}', '-p:Platform=Any CPU', '-getProperty:TargetPath', '-nologo'], cwd=consumer, text=True).strip()).with_suffix('.exe')
            if not winui.is_file():
                raise RuntimeError(f'Missing WinUI apphost: {winui}')
            for name in ('dxcompiler.dll', 'dxil.dll'):
                if (winui.parent / name).exists():
                    raise RuntimeError(f'WinUI precompiled consumer deployed {name}')
            run([str(winui), '--warp', '--frames=9'], consumer, evidence / 'character-winui-runtime.log', executable=True)
            run([str(winui), '--warp', '--host-test'], consumer, evidence / 'character-winui-host.log', executable=True)
            media_project = consumer / 'samples/VideoPlayer/VideoPlayer.csproj'
            media = Path(subprocess.check_output(['dotnet', 'msbuild', str(media_project), f'-p:Configuration={configuration}', '-p:Platform=Any CPU', '-getProperty:TargetPath', '-nologo'], cwd=consumer, text=True).strip()).with_suffix('.exe')
            for name in ('dxcompiler.dll', 'dxil.dll'):
                if (media.parent / name).exists():
                    raise RuntimeError(f'Media precompiled consumer deployed {name}')
            run([str(media), '--warp', '--test'], consumer, evidence / 'video-player.log', executable=True)
        (evidence / "samples.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
        print(f"PASS: all {len(manifest)} runnable samples consume packaged SDK {version} outside the checkout.")
        if public_feed:
            bytecode = list((consumer / "tools/ShaderConsumer/obj").rglob("Smoke.CS.cs_6_0.dxil"))
            if len(bytecode) != 1 or bytecode[0].read_bytes()[:4] != b"DXBC":
                raise RuntimeError("The public DXC build-task package did not produce its shader container.")
            print(f"PASS: NuGet.org-only fresh package cache, all ten SDK package IDs, and public DXC MSBuild task on .NET {runtime_major}.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--configuration", choices=("Debug", "Release"), default="Debug")
    parser.add_argument("--runtime-major", choices=("10", "11"), default="10")
    parser.add_argument("--version")
    parser.add_argument("--public-feed", action="store_true", help="Use only NuGet.org with a new package cache; wait for indexing and verify the public shader task.")
    args = parser.parse_args()
    if args.public_feed and not args.version:
        parser.error("--public-feed requires an explicit --version")
    qualify(args.configuration, args.runtime_major, args.version, args.public_feed)
