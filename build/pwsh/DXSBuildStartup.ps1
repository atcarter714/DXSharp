param (
    [string]$Argument1,
    [string]$Argument2,
    [string]$Argument3=""
)

# Show startup message:
Write-Host "----------------------------------------------"
Write-Host "DXSBuildStartup.ps1 :: Pre-Build script launched by " + $Argument2 + "`n"
Write-Host "----------------------------------------------"
Write-Host "Logging Path  :: $Argument1"
Write-Host "Assembly Name :: $Argument2"
if ($Argument3 -ne "") {
    Write-Host "Text      :: " + "`n" + $Argument3 + "`n"
    Write-Host " ____________________________________________" + "`n"
}


function LogDeployment {
  param(
        [string]$logPath,
        [string]$assemblyName,
        [string]$msg=""
    )
  
  # Get current date/time:
  $datetime = Get-Date
  
  # Create execution log text:
  $filetext = "`nDXSharp build startup tool executed by " + $assemblyName + " at " + $datetime
  if ($msg -ne "") {
    $filetext = $filetext + "`n" + $msg
  }
  
  $filetext | Out-File -filepath $logPath -Append
}

LogDeployment -logPath $Argument1 -assemblyName $Argument2 -msg $Argument3

