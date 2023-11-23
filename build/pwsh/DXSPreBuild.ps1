param (
    [string]$Argument1,     # Path to the log file
    [string]$Argument2,     # Name of the calling script
    [string]$Argument3=""   # Optional argument for a message/command ...
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
  $filetext = "DXSharp pre-build tool executed at " + $datetime + " ..." + "`n"
  
  # Add message if provided:
  if ($msg -ne "") {
    $filetext = $filetext + "MSBuild Message :: " + "`n" + $msg + "`n"
  }
  
  $filetext | Out-File -filepath $filepath -Append
}

LogPreBuildEvent $Argument1 $Argument2 $Argument3


# Show completion message:
Write-Host "DXS Pre-Build Tool: ($scriptName) completed."
 #>