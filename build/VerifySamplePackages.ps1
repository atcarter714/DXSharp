[CmdletBinding()]
param([ValidateSet('Debug', 'Release')] [string] $Configuration = 'Debug',
    [ValidateSet('10', '11')] [string] $RuntimeMajor = '10')
$ErrorActionPreference = 'Stop'
& python (Join-Path $PSScriptRoot 'verify_sample_packages.py') --configuration $Configuration --runtime-major $RuntimeMajor
if ($LASTEXITCODE -ne 0) { throw 'Isolated sample package qualification failed.' }
