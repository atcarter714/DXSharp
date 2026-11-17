[CmdletBinding()]
param([ValidateSet('Debug', 'Release')] [string] $Configuration = 'Debug')
$ErrorActionPreference = 'Stop'
& python (Join-Path $PSScriptRoot 'verify_sample_packages.py') --configuration $Configuration
if ($LASTEXITCODE -ne 0) { throw 'Isolated sample package qualification failed.' }
