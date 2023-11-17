param (
    [string]$Argument1,
    [string]$Argument2,
    [string]$Argument3=""
)

# Show startup message:
Write-Host "DXS Build Startup: Startup script (DXSBuildStartup.ps1) launched ..."
Write-Host "Arg0 :: $Argument1"
Write-Host "Arg1 :: $Argument2"
Write-Host "Arg3 :: $Argument3"


function LogDeployment {
  param( [string]$filepath, [string]$assemblyName, [string]$msg="" )
  
  # Get current date/time:
  $datetime = Get-Date

  # Create execution log text:
  $filetext = "`nDXSharp build startup tool executed by " + $assemblyName + " at " + $datetime + "`n"
  
  # Add message if provided:
  if ($msg -ne "") {
    $filetext = $filetext + "MSBuild Message :: " + "`n" + $msg + "`n"
  }
  
  $filetext | Out-File -filepath $filepath -Append
}

LogDeployment -filePath $Argument1 -assemblyName $Argument2 -msg $Argument3

