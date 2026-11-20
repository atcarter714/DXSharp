[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repoRoot = Split-Path $PSScriptRoot -Parent
$rid = if ($IsWindows) { 'win-x64' } elseif ($IsLinux) { 'linux-x64' } else { throw 'Restore qualification supports Windows and Linux x64.' }
function Invoke-Restore([string[]] $Arguments) {
    & dotnet restore @Arguments -m:1
    if ($LASTEXITCODE -ne 0) { throw "Restore isolation regression failed: dotnet restore $($Arguments -join ' ')" }
}
Push-Location $repoRoot
try {
    $before = @{}
    foreach ($directory in 'src', 'samples', 'tests') {
        Get-ChildItem (Join-Path $repoRoot $directory) -Recurse -File -Filter 'packages*.lock.json' | ForEach-Object {
            $before[$_.FullName] = (Get-FileHash $_.FullName -Algorithm SHA256).Hash
        }
    }
    foreach ($major in '10', '11') {
        $common = @("-p:DXSharpRuntimeMajor=$major")
        Invoke-Restore (@('tests/InteropQualification/InteropQualification.csproj', '--locked-mode') + $common)
        Invoke-Restore (@('tests/InteropQualification/InteropQualification.csproj', '-r', $rid, '-p:PublishAot=true') + $common)
        Invoke-Restore (@('samples/ModernCompute/ModernCompute.csproj', '-r', $rid, '-p:PublishAot=true') + $common)
        Invoke-Restore (@('tests/InteropQualification/InteropQualification.csproj', '--locked-mode') + $common)
        Invoke-Restore (@('samples/ModernCompute/ModernCompute.csproj', '--locked-mode') + $common)
        foreach ($path in $before.Keys) {
            if ((Get-FileHash $path -Algorithm SHA256).Hash -cne $before[$path]) {
                throw "AOT restore modified a canonical JIT lock file: $path"
            }
        }
    }
    Write-Host 'PASS: .NET 10/11 fixture and sample AOT restores preserve every canonical JIT lock file and subsequent locked restores.'
} finally { Pop-Location }
