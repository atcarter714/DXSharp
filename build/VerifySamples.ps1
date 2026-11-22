[CmdletBinding()]
param([ValidateSet('10', '11')] [string] $RuntimeMajor = '10')

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
if (-not $IsWindows) { throw 'The WARP sample qualification requires Windows.' }

Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    $results = "artifacts/sample-results/net$RuntimeMajor"
    New-Item -ItemType Directory -Force $results | Out-Null

    function Invoke-Sample([string] $Name, [string[]] $Arguments) {
        $forwarded = [System.Collections.Generic.List[string]]::new($Arguments)
        $index = $forwarded.IndexOf('--')
        if ($index -lt 0) { $index = $forwarded.Count }
        $forwarded.Insert($index, "-p:DXSharpRuntimeMajor=$RuntimeMajor")
        & dotnet @forwarded 2>&1 | Tee-Object -FilePath "$results/$Name.log"
        if ($LASTEXITCODE -ne 0) { throw "$Name failed with exit code $LASTEXITCODE." }
    }

    Invoke-Sample 'CharacterLab-native' @('run', '--project', 'samples/CharacterLab.Native', '-c', 'Debug', '--', '--warp', '--frames=9', '--resize-test')
    Invoke-Sample 'DepthStencil' @('run', '--project', 'samples/DepthStencil', '-c', 'Debug', '--', '--warp')

    Invoke-Sample 'ModernTriangle' @('run', '--project', 'samples/ModernTriangle', '-c', 'Debug', '--',
        '--warp', '--resize-test', "--evidence=artifacts/triangle-resize/net$RuntimeMajor")
    foreach ($sample in 'CharacterLab', 'BasicSample', 'HelloTexture', 'InstancedGrid', 'LitCube', 'PlanarShadows', 'StaticModelViewer') {
        Invoke-Sample $sample @('run', '--project', "samples/$sample", '-c', 'Debug', '--', '--warp', '--frames=4')
    }
    Invoke-Sample 'StaticModelViewer-textured' @('run', '--project', 'samples/StaticModelViewer', '-c', 'Debug', '--',
        '--warp', '--frames=4', '--model=samples/FleetBattler/Models/Freighter.gltf')
    Invoke-Sample 'FleetBattler' @('run', '--project', 'samples/FleetBattler', '-c', 'Debug', '--',
        '--warp', '--mute', '--ships=129', '--frames=7', '--resize-test')
    Invoke-Sample 'FleetBattler-views' @('run', '--project', 'samples/FleetBattler', '-c', 'Debug', '--',
        '--warp', '--mute', '--traffic=257', '--debris=513', '--frames=14', '--view-test')
    Invoke-Sample 'FleetBattler-dense-traffic' @('run', '--project', 'samples/FleetBattler', '-c', 'Debug', '--',
        '--warp', '--mute', '--traffic=8193', '--debris=2049', '--frames=3')
    Invoke-Sample 'BasicSample-buffered' @('run', '--project', 'samples/BasicSample', '-c', 'Debug', '--', '--warp', '--buffered', '--frames=7')
    Invoke-Sample 'ModernTriangle-buffered' @('run', '--project', 'samples/ModernTriangle', '-c', 'Debug', '--',
        '--warp', '--buffered', '--frames=8', '--resize-test', "--evidence=artifacts/triangle-resize/net$RuntimeMajor/buffered")
    Invoke-Sample 'NBodyGravity-numerical' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--verify', '--bodies=65', '--steps=5')
    Invoke-Sample 'NBodyGravity-window' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--bodies=129', '--frames=6')
    Invoke-Sample 'NBodyGravity-window-odd' @('run', '--project', 'samples/NBodyGravity', '-c', 'Debug', '--', '--warp', '--bodies=129', '--frames=7')
    Invoke-Sample 'DxrHelloWorld' @('run', '--project', 'samples/DxrHelloWorld', '-c', 'Debug', '--',
        '--warp', "--output=$results/dxr-hello-world.ppm")
    Invoke-Sample 'ModernCompute' @('run', '--project', 'samples/ModernCompute', '-c', 'Debug', '--',
        '--warp', '--directml', '--gemm', '--latest-shader-model')
    Invoke-Sample 'ModernCompute-system-runtime' @('run', '--project', 'samples/ModernCompute', '-c', 'Release', '--',
        '--warp', '--system-runtime', '--latest-shader-model')
    Invoke-Sample 'ModernCompute-publish' @('publish', 'samples/ModernCompute', '-c', 'Release', '-r', 'win-x64',
        '-p:PublishAot=true', '-o', "artifacts/modern-compute-aot/net$RuntimeMajor")
    & "./artifacts/modern-compute-aot/net$RuntimeMajor/ModernCompute.exe" --warp --directml --gemm --no-debug --latest-shader-model 2>&1 |
        Tee-Object -FilePath "$results/ModernCompute-aot.log"
    if ($LASTEXITCODE -ne 0) { throw "NativeAOT numerical qualification failed with exit code $LASTEXITCODE." }
} finally {
    Pop-Location
}
