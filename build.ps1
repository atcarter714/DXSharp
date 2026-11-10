[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug',
    [switch] $SkipTests,
    [switch] $RunGraphicsTests
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
if ($SkipTests -and $RunGraphicsTests) {
    throw 'SkipTests and RunGraphicsTests cannot be combined.'
}
if (-not $IsWindows -and -not $SkipTests) {
    throw 'Executing this Windows-targeted test assembly requires Windows. Use -SkipTests for cross-compilation.'
}

function Invoke-DotNet([string[]] $Arguments) {
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

Push-Location $PSScriptRoot
try {
    Invoke-DotNet -Arguments @('restore', 'DXSharp.sln', '--locked-mode', '-m:1')
    Invoke-DotNet -Arguments @('build', 'DXSharp.sln', '--configuration', $Configuration, '--no-restore', '-m:1', '-bl:artifacts/build.binlog')
    if (-not $SkipTests) {
        $resultsDirectory = Join-Path $PSScriptRoot "artifacts/test-results/$Configuration"
        New-Item -ItemType Directory -Force -Path $resultsDirectory | Out-Null
        $trxPath = Join-Path $resultsDirectory 'baseline.trx'
        if (Test-Path $trxPath) { Remove-Item $trxPath }
        Invoke-DotNet -Arguments @('test', 'tests/BasicTests/BasicTests.csproj', '--configuration', $Configuration,
            '--no-build', '--no-restore', '--filter', 'TestCategory=BuildBaseline',
            '--logger', 'trx;LogFileName=baseline.trx', '--results-directory', $resultsDirectory)
        [xml] $trx = Get-Content -Raw $trxPath
        if ([int] $trx.TestRun.ResultSummary.Counters.executed -lt 1) {
            throw 'The baseline test filter executed no tests.'
        }
        & (Join-Path $PSScriptRoot 'build/VerifyInterop.ps1') -Configuration $Configuration
        if ($RunGraphicsTests) {
            Invoke-DotNet -Arguments @('test', 'tests/BasicTests/BasicTests.csproj', '--configuration', $Configuration,
                '--no-build', '--no-restore', '--filter', 'TestCategory!=BuildBaseline',
                '--logger', 'trx;LogFileName=graphics.trx', '--results-directory', $resultsDirectory)
        }
    }
    Invoke-DotNet -Arguments @('pack', 'DXSharp.sln', '--configuration', $Configuration, '--no-build', '--no-restore', '-m:1')
    & (Join-Path $PSScriptRoot 'build/VerifyPackages.ps1') -Configuration $Configuration
} finally {
    Pop-Location
}
