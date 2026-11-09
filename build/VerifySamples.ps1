[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
if (-not $IsWindows) { throw 'The WARP sample qualification requires Windows.' }

Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    New-Item -ItemType Directory -Force artifacts/sample-results | Out-Null

    function Invoke-Sample([string] $Name, [string[]] $Arguments) {
        & dotnet @Arguments 2>&1 | Tee-Object -FilePath "artifacts/sample-results/$Name.log"
        if ($LASTEXITCODE -ne 0) { throw "$Name failed with exit code $LASTEXITCODE." }
    }

    Invoke-Sample 'ModernTriangle' @('run', '--project', 'samples/ModernTriangle', '-c', 'Debug', '--',
        '--warp', '--resize-test', '--evidence=artifacts/triangle-resize')
    foreach ($sample in 'HelloTexture', 'InstancedGrid', 'LitCube', 'PlanarShadows') {
        Invoke-Sample $sample @('run', '--project', "samples/$sample", '-c', 'Debug', '--', '--warp', '--frames=4')
    }
    Invoke-Sample 'ModernTriangle-buffered' @('run', '--project', 'samples/ModernTriangle', '-c', 'Debug', '--',
        '--warp', '--buffered', '--frames=8', '--resize-test', '--evidence=artifacts/triangle-resize/buffered')
    Invoke-Sample 'NBodyGravity-numerical' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--verify', '--bodies=65', '--steps=5')
    Invoke-Sample 'NBodyGravity-window' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--bodies=129', '--frames=6')
    Invoke-Sample 'DxrHelloWorld' @('run', '--project', 'samples/DxrHelloWorld', '-c', 'Debug', '--',
        '--warp', '--output=artifacts/sample-results/dxr-hello-world.ppm')
    Invoke-Sample 'ModernCompute' @('run', '--project', 'samples/ModernCompute', '-c', 'Debug', '--', '--warp', '--directml', '--gemm')
    Invoke-Sample 'ModernCompute-publish' @('publish', 'samples/ModernCompute', '-c', 'Release', '-r', 'win-x64',
        '-p:PublishAot=true', '-o', 'artifacts/modern-compute-aot')
    & ./artifacts/modern-compute-aot/ModernCompute.exe --warp --directml --gemm --no-debug 2>&1 |
        Tee-Object -FilePath artifacts/sample-results/ModernCompute-aot.log
    if ($LASTEXITCODE -ne 0) { throw "NativeAOT numerical qualification failed with exit code $LASTEXITCODE." }
} finally {
    Pop-Location
}
