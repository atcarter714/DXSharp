[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug',
    [ValidateSet('10', '11')] [string] $RuntimeMajor = '10'
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
    foreach ($name in @('DXSharp', 'DXSharp.Dxc', 'DXSharp.Framework', 'DXSharp.Framework.Desktop', 'DXSharp.Framework.Wpf', 'DXSharp.Framework.WinUI', 'DXSharp.Native', 'DXSharp.Assets', 'DXSharp.Core', 'DXSharp.DirectML')) {
        $path = Join-Path $packageDirectory "$name.$version.nupkg"
        if (-not (Test-Path $path)) { throw "Missing package: $path" }
        $archive = [System.IO.Compression.ZipFile]::OpenRead($path)
        try {
            $reader = [System.IO.StreamReader]::new($archive.GetEntry("$name.nuspec").Open())
            try { [xml] $metadata = $reader.ReadToEnd() } finally { $reader.Dispose() }
            $frameworkReferences = @($metadata.SelectNodes("//*[local-name()='frameworkReference']") | ForEach-Object { $_.GetAttribute('name') })
            $winuiDependencies = @($metadata.SelectNodes("//*[local-name()='dependency']") | Where-Object { $_.GetAttribute('id') -like 'Microsoft.WindowsAppSDK*' })
            if ($name -ne 'DXSharp.Framework.WinUI' -and $winuiDependencies.Count -gt 0) { throw "Optional WinUI dependency leaked into $name." }
            if ($name -eq 'DXSharp.Framework.WinUI' -and $winuiDependencies.Count -eq 0) { throw 'The WinUI adapter lost its pinned Windows App SDK dependency.' }
            if ($name -eq 'DXSharp.Framework.Wpf' -and
                ('Microsoft.WindowsDesktop.App.WPF' -notin $frameworkReferences -or 'Microsoft.WindowsDesktop.App.WindowsForms' -in $frameworkReferences)) {
                throw 'The WPF package must declare WPF without acquiring Windows Forms.'
            }
            if ($name -in @('DXSharp.Native', 'DXSharp.Core', 'DXSharp.Framework', 'DXSharp.Assets') -and
                @($frameworkReferences | Where-Object { $_ -like 'Microsoft.WindowsDesktop*' }).Count -gt 0) {
                throw "Optional desktop framework leaked into $name."
            }
            $assemblies = @($archive.Entries | Where-Object FullName -CLike "lib/*/$name.dll")
            if ($assemblies.Count -eq 0) { throw "No library assembly found in $path" }
            if ($name -in @('DXSharp.Native', 'DXSharp.Core', 'DXSharp.DirectML', 'DXSharp.Framework.Desktop', 'DXSharp.Framework.Wpf', 'DXSharp.Framework.WinUI', 'DXSharp.Assets', 'DXSharp.Framework')) {
                foreach ($major in '10', '11') {
                    # NuGet normalizes the trailing zero of the Windows platform version.
                    $framework = if ($name -eq 'DXSharp.Assets') { "net$major.0" } else { "net$major.0-windows10.0.26100" }
                    if ($null -eq $archive.GetEntry("lib/$framework/$name.dll")) { throw "Missing $framework asset in $path" }
                }
            }
            foreach ($assembly in $assemblies) {
                $docPath = [System.IO.Path]::ChangeExtension($assembly.FullName, '.xml')
                $entry = $archive.GetEntry($docPath)
                if ($null -eq $entry) { throw "Missing $docPath in $path" }
                $reader = [System.IO.StreamReader]::new($entry.Open())
                try {
                    [xml] $documentation = $reader.ReadToEnd()
                    if ($documentation.SelectSingleNode('/doc/assembly/name').InnerText -cne $name -or
                        $null -eq $documentation.SelectSingleNode('/doc/members/member')) {
                        throw "Empty or mismatched XML documentation: $docPath in $path"
                    }
                } finally { $reader.Dispose() }
            }
        } finally { $archive.Dispose() }
    }
    $toolArchive = [System.IO.Compression.ZipFile]::OpenRead((Join-Path $packageDirectory "DXSharp.Tools.Builds.$version.nupkg"))
    try {
        foreach ($entry in 'tasks/netstandard2.0/DXSharp.Tools.Builds.dll', 'buildTransitive/DXSharp.Tools.Builds.targets') {
            if ($null -eq $toolArchive.GetEntry($entry)) { throw "Missing shader build asset: $entry" }
        }
    } finally { $toolArchive.Dispose() }
    @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net${RuntimeMajor}.0-windows10.0.26100.0</TargetFramework>
    <EnableWindowsTargeting>true</EnableWindowsTargeting>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWindowsForms>true</UseWindowsForms>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <PublishSingleFile>true</PublishSingleFile>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="DXSharp" Version="$version" />
    <PackageReference Include="DXSharp.Dxc" Version="$version" />
    <PackageReference Include="DXSharp.Framework" Version="$version" />
    <PackageReference Include="DXSharp.Framework.Desktop" Version="$version" />
    <PackageReference Include="DXSharp.DirectML" Version="$version" />
    <PackageReference Include="DXSharp.Assets" Version="$version" />
  </ItemGroup>
</Project>
"@ | Set-Content (Join-Path $consumerDirectory 'Consumer.csproj')
    Copy-Item (Join-Path $repoRoot 'samples/ModernCompute/Program.cs') (Join-Path $consumerDirectory 'Program.cs')
    @'
internal static class PackageSurfaces {
    internal static readonly System.Type[] Types = [typeof(DXSharp.Direct3D12.IDevice), typeof(DXSharp.Dxc.ShaderBuilder),
        typeof(DXSharp.Graphics.GraphicsDevice), typeof(Windows.Win32.Graphics.Direct3D12.ID3D12Device16), typeof(Windows.Win32.Graphics.Direct3D12.ID3D12GraphicsCommandList12),
        typeof(Windows.Win32.Graphics.Direct2D.ID2D1DeviceContext), typeof(Windows.Win32.Media.Audio.XAudio2.IXAudio2),
        typeof(DXSharp.Assets.GltfModel)];
}
'@ | Set-Content (Join-Path $consumerDirectory 'PackageSurfaces.cs')
    @"
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$([System.Security.SecurityElement]::Escape($packageDirectory))" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local">
      <package pattern="DXSharp*" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
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
            'D3D12/D3D12Core.dll' = 'microsoft.direct3d.d3d12/1.620.2/build/native/bin/x64/D3D12Core.dll'
            'D3D12/D3D12StateObjectCompiler.dll' = 'microsoft.direct3d.d3d12/1.620.2/build/native/bin/x64/D3D12StateObjectCompiler.dll'
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
        if ($IsWindows) {
            Write-Host "Running DXC, WARP and DirectML from the published package consumer."
            & (Join-Path $publish 'Consumer.exe') --warp --no-debug --directml --gemm
            if ($LASTEXITCODE -ne 0) { throw 'Published package consumer GPU smoke test failed.' }
        }
        foreach ($debugLayer in @('D3D12/d3d12SDKLayers.dll', 'DirectML.Debug.dll')) {
            if (Test-Path (Join-Path $publish $debugLayer)) { throw "Debug layer leaked into publish: $debugLayer" }
        }
    } finally {
        Pop-Location
    }
} finally {
    Remove-Item -Recurse -Force $consumerDirectory
}
