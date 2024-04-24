#runme: powershell -ExecutionPolicy Bypass -File .\read-pipelines-init.ps1
param(
    [string]$iniFile=""
)
$iniFile = $iniFile -eq "" ? "..\pipelines.init" : $iniFile
Write-Host "Reading file: $iniFile"

$iniContent = Get-Content $iniFile

$winSdkVersionEntry = $iniContent | Where-Object { $_ -match "WinSDKVersion" }
$dotNetVersionsEntry = $iniContent | Where-Object { $_ -match "DotNetVersions" }

$winSdkVersion = ($winSdkVersionEntry -split '=')[1].Trim()
$dotNetVersions = ($dotNetVersionsEntry -split '=')[1].Trim()

$dotNetVersionArray = $dotNetVersions -split ';'

$targetFrameworkStrings = @()

foreach ($ver in $dotNetVersionArray) {
    $targetFrameworkStrings += "$ver-windows$winSdkVersion"
}

Write-Host "##vso[task.setvariable variable=WinSdkVersion]$winSdkString"
Write-Host "##vso[task.setvariable variable=TargetFrameworkStrings]$targetFrameworkStrings"


return $targetFrameworkStrings