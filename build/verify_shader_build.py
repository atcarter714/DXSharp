"""Qualify real DXC compilation, incremental state, includes, diagnostics and isolated build-tool packaging."""
import argparse
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import xml.etree.ElementTree as ET


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--configuration', default='Release')
    parser.add_argument('--runtime-major', default='10')
    parser.add_argument('--compiler')
    parser.add_argument('--source-task', action='store_true')
    args = parser.parse_args()
    root = Path(__file__).resolve().parent.parent
    version = subprocess.check_output(['dotnet', 'msbuild', str(root/'src/DXSharp.Core/DXSharp.Core.csproj'), '-getProperty:PackageVersion', '-nologo'], text=True).strip()
    with tempfile.TemporaryDirectory(prefix='dxsharp shader build ') as temporary:
        consumer = Path(temporary)
        shutil.copyfile(root/'global.json', consumer/'global.json')
        project = ET.Element('Project', Sdk='Microsoft.NET.Sdk')
        properties = ET.SubElement(project, 'PropertyGroup')
        for key, value in {'TargetFramework': f'net{args.runtime_major}.0', 'DXSharpEmbedShaders': 'false',
                           'DXSharpShaderDebug': 'false', 'NuGetAudit': 'false'}.items():
            ET.SubElement(properties, key).text = value
        if args.compiler:
            ET.SubElement(properties, 'DXSharpDxcToolPath').text = args.compiler
        if args.source_task:
            ET.SubElement(properties, 'DXSharpShaderTaskAssembly').text = str(root/f'artifacts/bin/DXSharp.Tools.Builds/{args.configuration}/netstandard2.0/DXSharp.Tools.Builds.dll')
            ET.SubElement(project, 'Import', Project=str(root/'build/msbuild/DXSharp.Shaders.targets'))
        else:
            group = ET.SubElement(project, 'ItemGroup')
            ET.SubElement(group, 'PackageReference', Include='DXSharp.Tools.Builds', Version=f'[{version}]', PrivateAssets='all')
        group = ET.SubElement(project, 'ItemGroup')
        for entry, profile in [('VS', 'vs_6_0'), ('PS', 'ps_6_0')]:
            ET.SubElement(group, 'DXSharpShader', Include='source with spaces/main.hlsl', EntryPoint=entry, Profile=profile,
                          Defines='FACTOR=$(Factor)', IncludeDirectories='source with spaces')
        ET.SubElement(properties, 'Factor').text = '1'
        ET.ElementTree(project).write(consumer/'Test.csproj', encoding='utf-8')
        config = ET.Element('configuration'); sources = ET.SubElement(config, 'packageSources'); ET.SubElement(sources, 'clear')
        ET.SubElement(sources, 'add', key='local', value=str(root/f'artifacts/packages/{args.configuration}'))
        if not args.source_task:
            ET.SubElement(sources, 'add', key='nuget.org', value='https://api.nuget.org/v3/index.json')
            mapping = ET.SubElement(config, 'packageSourceMapping')
            for key, pattern in [('local', 'DXSharp.*'), ('nuget.org', '*')]:
                ET.SubElement(ET.SubElement(mapping, 'packageSource', key=key), 'package', pattern=pattern)
        ET.ElementTree(config).write(consumer/'NuGet.Config', encoding='utf-8')
        folder = consumer/'source with spaces'; (folder/'nested').mkdir(parents=True)
        (folder/'main.hlsl').write_text('#include "nested/outer.hlsli"\nfloat4 VS(uint id:SV_VertexID):SV_Position{return float4(id,0,0,1);}\nfloat4 PS():SV_Target{return Shade()*FACTOR;}\n')
        (folder/'nested/outer.hlsli').write_text('#include "../color.hlsli"\nfloat4 Shade(){return COLOR;}\n')
        include = folder/'color.hlsli'; valid = '#define COLOR float4(1,0,0,1)\n'; include.write_text(valid)
        logs = []
        def run(*arguments, success=True):
            result = subprocess.run(['dotnet', *arguments], cwd=consumer, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
            logs.append(result.stdout)
            if (result.returncode == 0) != success:
                raise RuntimeError(result.stdout)
            return result.stdout
        run('restore', 'Test.csproj', '--packages', str(consumer/'packages'))
        def build(*extra, success=True):
            return run('build', 'Test.csproj', '--no-restore', '-c', args.configuration, '-p:UseSharedCompilation=false', '-nodeReuse:false', *extra, success=success)
        first = build(); containers = sorted((consumer/'obj').rglob('*.dxil'))
        if len(containers) != 2 or any(p.read_bytes()[:4] != b'DXBC' for p in containers):
            raise RuntimeError('Expected two real shader containers')
        first_times = [p.stat().st_mtime_ns for p in containers]
        second = build()
        if second.count('DXC up-to-date:') != 2 or first_times != [p.stat().st_mtime_ns for p in containers]:
            raise RuntimeError('Unchanged shaders were recompiled')
        include.write_text('#define COLOR float4(0,1,0,1)\n')
        if build().count('DXC compiled:') != 2:
            raise RuntimeError('Nested include modification did not invalidate both compilation units')
        if build('-p:Factor=2').count('DXC compiled:') != 2:
            raise RuntimeError('Define change did not invalidate compilation')
        include.write_text('#error intentional shader failure\n')
        failure = build(success=False)
        if 'intentional shader failure' not in failure or 'DXSHADER' not in failure:
            raise RuntimeError('Shader failure lost diagnostic information')
        include.unlink(); build(success=False)
        include.write_text(valid); build('-p:DXSharpShaderDebug=true')
        if len(list((consumer/'obj').rglob('*.dxil.pdb'))) != 2:
            raise RuntimeError('Missing debug PDBs')
        manifest = ET.parse(containers[0].with_name(containers[0].name+'.xml'))
        if not any(x.attrib['Path'].endswith('color.hlsli') for x in manifest.findall('Dependency')):
            raise RuntimeError('Transitive include missing from shader manifest')
        run('clean', 'Test.csproj', '-c', args.configuration)
        if any(p.exists() for p in containers):
            raise RuntimeError('Clean left shader intermediates behind')
        evidence = root/f'artifacts/shader-build/{args.configuration}/net{args.runtime_major}'
        evidence.mkdir(parents=True, exist_ok=True)
        (evidence/'qualification.log').write_text('\n'.join(logs))
        print('PASS: shader package; two entry points; paths with spaces; unchanged/define/transitive-include rebuilds; errors; deleted includes; PDBs; manifests; clean.')


if __name__ == '__main__':
    main()
