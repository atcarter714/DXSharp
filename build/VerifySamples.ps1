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
    foreach ($sample in 'HelloTexture', 'InstancedGrid', 'LitCube', 'PlanarShadows', 'StaticModelViewer') {
        Invoke-Sample $sample @('run', '--project', "samples/$sample", '-c', 'Debug', '--', '--warp', '--frames=4')
    }
    Invoke-Sample 'StaticModelViewer-textured' @('run', '--project', 'samples/StaticModelViewer', '-c', 'Debug', '--',
        '--warp', '--frames=4', '--model=samples/FleetBattler/Models/Freighter.gltf')
    Invoke-Sample 'FleetBattler' @('run', '--project', 'samples/FleetBattler', '-c', 'Debug', '--',
        '--warp', '--mute', '--ships=129', '--frames=7', '--resize-test')
    Invoke-Sample 'FleetBattler-views' @('run', '--project', 'samples/FleetBattler', '-c', 'Debug', '--',
        '--warp', '--mute', '--traffic=257', '--debris=513', '--frames=12', '--view-test')
    Invoke-Sample 'FleetBattler-dense-traffic' @('run', '--project', 'samples/FleetBattler', '-c', 'Debug', '--',
        '--warp', '--mute', '--traffic=8193', '--debris=2049', '--frames=3')
    Invoke-Sample 'ModernTriangle-buffered' @('run', '--project', 'samples/ModernTriangle', '-c', 'Debug', '--',
        '--warp', '--buffered', '--frames=8', '--resize-test', '--evidence=artifacts/triangle-resize/buffered')
    Invoke-Sample 'NBodyGravity-numerical' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--verify', '--bodies=65', '--steps=5')
    Invoke-Sample 'NBodyGravity-window' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--bodies=129', '--frames=6')
    Invoke-Sample 'NBodyGravity-window-odd' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--bodies=129', '--frames=7')
    Invoke-Sample 'DxrHelloWorld' @('run', '--project', 'samples/DxrHelloWorld', '-c', 'Debug', '--',
        '--warp', '--output=artifacts/sample-results/dxr-hello-world.ppm')
    Invoke-Sample 'ModernCompute' @('run', '--project', 'samples/ModernCompute', '-c', 'Debug', '--',
        '--warp', '--directml', '--gemm', '--latest-shader-model')
    Invoke-Sample 'ModernCompute-system-runtime' @('run', '--project', 'samples/ModernCompute', '-c', 'Release', '--',
        '--warp', '--system-runtime', '--latest-shader-model')
    Invoke-Sample 'ModernCompute-publish' @('publish', 'samples/ModernCompute', '-c', 'Release', '-r', 'win-x64',
        '-p:PublishAot=true', '-o', 'artifacts/modern-compute-aot')
    & ./artifacts/modern-compute-aot/ModernCompute.exe --warp --directml --gemm --no-debug --latest-shader-model 2>&1 |
        Tee-Object -FilePath artifacts/sample-results/ModernCompute-aot.log
    if ($LASTEXITCODE -ne 0) { throw "NativeAOT numerical qualification failed with exit code $LASTEXITCODE." }
} finally {
    Pop-Location
}
