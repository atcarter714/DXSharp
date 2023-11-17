param (
    [string]$Argument1,
    [string]$Argument2,
    [string]$Argument3="",
)

$scriptName = "DXSPreBuild.ps1"

# Show startup message:
Write-Host "DXS Pre-Build Tool: ($scriptName) launched ..."



# LogDeployment $args[0] $args[1] $args[2]
function LogPreBuildEvent {
  param( [string]$filepath, [string]$callerName, [string]$msg="" )
  
  # Get current date/time:
  $datetime = Get-Date

  # Create execution log text:
  $filetext = "DXSharp pre-build tool executed at " + $datetime + " ..." + '\n'
  
  # Add message if provided:
  if ($msg -ne "") {
    $filetext = $filetext + "MSBuild Message :: " + '\n' + $msg + '\n'
  }
  
  $filetext | Out-File -filepath $filepath -Append
}
