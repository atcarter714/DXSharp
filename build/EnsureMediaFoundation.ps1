[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
if (-not $IsWindows) { return }
if (Get-Command Get-WindowsFeature -ErrorAction SilentlyContinue) {
    $feature = Get-WindowsFeature -Name Server-Media-Foundation
    if ($feature -and -not $feature.Installed) {
        $result = Install-WindowsFeature -Name Server-Media-Foundation
        if (-not $result.Success -or $result.RestartNeeded -eq 'Yes') { throw 'Media Foundation installation did not complete without a reboot.' }
    }
}
