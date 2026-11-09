[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repoRoot = Split-Path $PSScriptRoot -Parent
$packageDirectory = Join-Path $repoRoot "artifacts/packages/$Configuration"
$consumerDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "dxsharp-package-consumer-$([Guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $consumerDirectory | Out-Null
try {
    Copy-Item (Join-Path $repoRoot 'global.json') $consumerDirectory
    $version = (& dotnet msbuild (Join-Path $repoRoot 'src/DXSharp/DXSharp.csproj') -getProperty:PackageVersion -nologo | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($version)) {
        throw 'Unable to evaluate the package version.'
    }
    foreach ($name in @('DXSharp', 'DXSharp.Dxc', 'DXSharp.Framework', 'DXSharp.Framework.Desktop', 'DXSharp.Core', 'DXSharp.DirectML')) {
        if (-not (Test-Path (Join-Path $packageDirectory "$name.$version.nupkg"))) {
            throw "Missing package: $name.$version.nupkg"
        }
    }
    @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0-windows10.0.26100.0</TargetFramework>
    <EnableWindowsTargeting>true</EnableWindowsTargeting>
    <UseWindowsForms>true</UseWindowsForms>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <PublishSingleFile>true</PublishSingleFile>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="DXSharp.Framework" Version="$version" />
    <PackageReference Include="DXSharp.Framework.Desktop" Version="$version" />
    <PackageReference Include="DXSharp.DirectML" Version="$version" />
  </ItemGroup>
</Project>
"@ | Set-Content (Join-Path $consumerDirectory 'Consumer.csproj')
    @'
System.Console.WriteLine(typeof(DXSharp.Direct3D12.IDevice).Assembly.FullName);
System.Console.WriteLine(typeof(DXSharp.Dxc.ShaderBuilder).Assembly.FullName);
System.Console.WriteLine(typeof(DXSharp.Graphics.GraphicsDevice).Assembly.FullName);
System.Console.WriteLine(typeof(DXSharp.Framework.Desktop.DesktopGame).Assembly.FullName);
System.Console.WriteLine(typeof(DXSharp.MachineLearning.DirectMlDevice).Assembly.FullName);
'@ | Set-Content (Join-Path $consumerDirectory 'Program.cs')
    @"
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$([System.Security.SecurityElement]::Escape($packageDirectory))" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@ | Set-Content (Join-Path $consumerDirectory 'NuGet.Config')
    Push-Location $consumerDirectory
    try {
        # A private cache prevents a previously built package with the same preview version masking this build.
        & dotnet restore Consumer.csproj --packages (Join-Path $consumerDirectory 'packages')
        if ($LASTEXITCODE -ne 0) { throw 'Package consumer restore failed.' }
        & dotnet build Consumer.csproj --configuration $Configuration --no-restore
        if ($LASTEXITCODE -ne 0) { throw 'Package consumer build failed.' }
        $output = (& dotnet msbuild Consumer.csproj "-p:Configuration=$Configuration" -getProperty:TargetDir -nologo | Out-String).Trim()
        if ($LASTEXITCODE -ne 0 -or -not (Test-Path $output)) { throw 'Unable to locate the package consumer output.' }
        $native = @{
            'D3D12/D3D12Core.dll' = 'microsoft.direct3d.d3d12/1.620.1/build/native/bin/x64/D3D12Core.dll'
            'D3D12/D3D12StateObjectCompiler.dll' = 'microsoft.direct3d.d3d12/1.620.1/build/native/bin/x64/D3D12StateObjectCompiler.dll'
            'dxcompiler.dll' = 'microsoft.direct3d.dxc/1.9.2609.5/build/native/bin/x64/dxcompiler.dll'
            'dxil.dll' = 'microsoft.direct3d.dxc/1.9.2609.5/build/native/bin/x64/dxil.dll'
            'DirectML.dll' = 'microsoft.ai.directml/1.15.4/bin/x64-win/DirectML.dll'
        }
        function Test-NativeOutput([string] $Directory) {
            foreach ($relative in $native.Keys) {
                $actual = Join-Path $Directory $relative
                $expected = Join-Path $consumerDirectory "packages/$($native[$relative])"
                if (-not (Test-Path $actual) -or (Get-FileHash $actual).Hash -ne (Get-FileHash $expected).Hash) {
                    throw "Missing or incorrect native package asset: $actual"
                }
            }
        }
        Test-NativeOutput $output
        $publish = Join-Path $consumerDirectory 'publish'
        & dotnet publish Consumer.csproj --configuration $Configuration --no-restore --output $publish
        if ($LASTEXITCODE -ne 0) { throw 'Package consumer single-file publish failed.' }
        Test-NativeOutput $publish
        foreach ($debugLayer in @('D3D12/d3d12SDKLayers.dll', 'DirectML.Debug.dll')) {
            if (Test-Path (Join-Path $publish $debugLayer)) { throw "Debug layer leaked into publish: $debugLayer" }
        }
    } finally {
        Pop-Location
    }
} finally {
    Remove-Item -Recurse -Force $consumerDirectory
}
