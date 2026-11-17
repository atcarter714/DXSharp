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
    $assembly = (& dotnet msbuild $nativeProject "-p:Configuration=$Configuration" -getProperty:TargetPath -nologo | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $assembly)) { throw 'Build DXSharp.Native before checking API coverage.' }
    New-Item -ItemType Directory -Force 'artifacts/api-coverage' | Out-Null
    $arguments = @('run', '--project', 'tools/ApiCoverage', '--no-build', '--no-restore', '-c', $Configuration,
        '--', (Join-Path $metadataRoot 'Windows.Win32.winmd'), $assembly, "artifacts/api-coverage/$Configuration.json")
    & dotnet @arguments
    if ($LASTEXITCODE -ne 0) { throw 'Public native API coverage regressed; inspect artifacts/api-coverage.' }
} finally {
    Pop-Location
}
