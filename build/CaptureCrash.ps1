[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $Executable,
    [Parameter(Mandatory)] [string] $ProcDumpPath,
    [string[]] $Arguments = @(),
    [string] $OutputDirectory = (Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/crashes')
)
$ErrorActionPreference = 'Stop'
if (-not $IsWindows) { throw 'Crash capture requires Windows.' }
$executablePath = (Resolve-Path $Executable).Path
$procDump = (Resolve-Path $ProcDumpPath).Path
$directory = [System.IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force $directory | Out-Null
@{
    Executable = $executablePath
    Arguments = $Arguments
    StartedUtc = [DateTime]::UtcNow.ToString('O')
    OS = [System.Runtime.InteropServices.RuntimeInformation]::OSDescription
    Architecture = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString()
    SHA256 = (Get-FileHash $executablePath -Algorithm SHA256).Hash
    DotNet = (& dotnet --info | Out-String)
} | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $directory 'session.json')
# Full dump on an unhandled exception, launching this executable only. ProcDump
# handles its own first-run license prompt. No machine-wide crash settings change.
& $procDump -ma -e -x $directory $executablePath @Arguments 2>&1 | Tee-Object (Join-Path $directory 'procdump.log')
if ($LASTEXITCODE -ne 0) { throw "ProcDump exited with code $LASTEXITCODE. See $directory." }
