[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repoRoot = Split-Path $PSScriptRoot -Parent
Push-Location $repoRoot
try {
    $nativeProject = 'src/DXSharp.Native/DXSharp.Native.csproj'
    $metadataRoot = (& dotnet msbuild $nativeProject -getProperty:PkgMicrosoft_Windows_SDK_Win32Metadata -nologo | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($metadataRoot)) {
        throw 'Restore DXSharp.Native before checking API coverage.'
    }
    $agilityRoot = (& dotnet msbuild $nativeProject -getProperty:PkgMicrosoft_Direct3D_D3D12 -nologo | Out-String).Trim()
    $frameworks = (& dotnet msbuild $nativeProject -getProperty:TargetFrameworks -nologo | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($agilityRoot) -or [string]::IsNullOrWhiteSpace($frameworks)) {
        throw 'Unable to resolve the retail metadata provider and target frameworks.'
    }
    New-Item -ItemType Directory -Force "artifacts/api-coverage/$Configuration" | Out-Null
    # The solution maps build-host tools to AnyCPU even inside an x64 MSVC shell.
    $tool = (& dotnet msbuild 'tools/ApiCoverage/ApiCoverage.csproj' '-p:Platform=AnyCPU' "-p:Configuration=$Configuration" -getProperty:TargetPath -nologo | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $tool)) { throw 'Build ApiCoverage before checking API coverage.' }
    $metadata = (Join-Path $metadataRoot 'Windows.Win32.winmd') + ';' + (Join-Path $agilityRoot 'metadata/Microsoft.Direct3D.D3D12.winmd')
    foreach ($framework in $frameworks.Split(';', [StringSplitOptions]::RemoveEmptyEntries)) {
        $assembly = (& dotnet msbuild $nativeProject "-p:Configuration=$Configuration" "-p:TargetFramework=$framework" -getProperty:TargetPath -nologo | Out-String).Trim()
        if ($LASTEXITCODE -ne 0 -or -not (Test-Path $assembly)) { throw "Build DXSharp.Native for $framework before checking API coverage." }
        & dotnet $tool $metadata $assembly "artifacts/api-coverage/$Configuration/$framework.json"
        if ($LASTEXITCODE -ne 0) { throw 'Public native API coverage regressed; inspect artifacts/api-coverage.' }
        & python (Join-Path $PSScriptRoot 'verify_api_coverage.py') --tool $tool --metadata $metadata --assembly $assembly --report "artifacts/api-coverage/$Configuration/$framework.json"
        if ($LASTEXITCODE -ne 0) { throw 'API coverage failure-path qualification failed.' }
    }
} finally {
    Pop-Location
}
