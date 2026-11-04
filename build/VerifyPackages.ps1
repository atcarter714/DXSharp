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
    foreach ($name in @('DXSharp', 'DXSharp.Dxc', 'DXSharp.Framework')) {
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
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="DXSharp.Framework" Version="$version" />
  </ItemGroup>
</Project>
"@ | Set-Content (Join-Path $consumerDirectory 'Consumer.csproj')
    @'
System.Console.WriteLine(typeof(DXSharp.Direct3D12.IDevice).Assembly.FullName);
System.Console.WriteLine(typeof(DXSharp.Dxc.ShaderBuilder).Assembly.FullName);
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
    } finally {
        Pop-Location
    }
} finally {
    Remove-Item -Recurse -Force $consumerDirectory
}
