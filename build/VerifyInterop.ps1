[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')] [string] $Configuration = 'Debug',
    [ValidateSet('x64')] [string] $Architecture = 'x64',
    [switch] $PublishAot
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repoRoot = Split-Path $PSScriptRoot -Parent
function Invoke-Checked([string] $Command, [string[]] $Arguments) {
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Command failed with exit code $LASTEXITCODE." }
}
Push-Location $repoRoot
try {
    $nativeDirectory = Join-Path $repoRoot "artifacts/native/$Architecture"
    New-Item -ItemType Directory -Force $nativeDirectory | Out-Null
    $source = Join-Path $repoRoot 'tests/InteropQualification/native/fixture.cpp'
    if ($IsWindows) {
        # Run from a Developer PowerShell, with cl.exe configured for Architecture.
        $fixture = Join-Path $nativeDirectory 'dxsharp_abi.dll'
        Invoke-Checked 'cl.exe' @('/nologo', '/std:c++17', '/EHsc', '/LD', '/O2', $source,
            "/Fo$nativeDirectory/fixture.obj", '/link', "/OUT:$fixture", "/IMPLIB:$nativeDirectory/dxsharp_abi.lib")
        $rid = "win-$Architecture"
    } elseif ($IsLinux -and $Architecture -eq 'x64') {
        $fixture = Join-Path $nativeDirectory 'libdxsharp_abi.so'
        Invoke-Checked 'g++' @('-std=c++17', '-shared', '-fPIC', '-O2', $source, '-o', $fixture)
        $rid = 'linux-x64'
    } else { throw 'This script qualifies Windows x64/x86/ARM64 and Linux x64.' }
    $project = 'tests/InteropQualification/InteropQualification.csproj'
    Invoke-Checked 'dotnet' @('restore', $project, '--locked-mode', '-m:1')
    Invoke-Checked 'dotnet' @('build', $project, '-c', $Configuration, '--no-restore', '-m:1', "-p:DXSharpNativeFixture=$fixture")
    $managed = (& dotnet msbuild $project "-p:Configuration=$Configuration" -getProperty:TargetPath -nologo | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $managed)) { throw 'Unable to locate the built qualification assembly.' }
    Invoke-Checked 'dotnet' @($managed)
    if ($PublishAot) {
        $publish = Join-Path $repoRoot "artifacts/qualification-aot/$rid"
        Invoke-Checked 'dotnet' @('publish', $project, '-m:1', '-c', $Configuration, '-r', $rid, '-p:PublishAot=true',
            "-p:DXSharpNativeFixture=$fixture", '-o', $publish)
        Copy-Item $fixture $publish -Force
        $executable = Join-Path $publish $(if ($IsWindows) { 'InteropQualification.exe' } else { 'InteropQualification' })
        Invoke-Checked $executable @()
    }
} finally { Pop-Location }
