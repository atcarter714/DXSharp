param (
    [string]$outputDir = "build/dxc",
    [string]$version = "latest",
    [string]$D3DDebug = "false"
)


# Define the GitHub repository owner and name
$repoOwner = "microsoft"
$repoName = "DirectXShaderCompiler"

# Your location to download and unzip the files
$downloadPath = Join-Path -Path $(Get-Location) -ChildPath ($outputDir + "\download")


# Define the URL to the GitHub API for fetching the latest release
$url = "https://api.github.com/repos/$repoOwner/$repoName/releases/$version"

# Call the GitHub API
$response = Invoke-WebRequest -Uri $url -UseBasicParsing

# Convert the JSON response to a PowerShell object
$object = ConvertFrom-Json -InputObject $response.Content

# The download URL to the latest release will be the `browser_download_url` field in each asset
$object.assets | ForEach-Object {
    Write-Output $_.browser_download_url
}


# Get the download URLs of the desired binary and debug package
$binaryUrl = $object.assets | Where-Object { $_.name -like "*dxc_*.zip" } | Select-Object -ExpandProperty browser_download_url
$debugUrl = $object.assets | Where-Object { $_.name -like "*pdb_*.zip" } | Select-Object -ExpandProperty browser_download_url

# Download the binary package
$binaryFile = Join-Path -Path $downloadPath -ChildPath "$($repoName)_$(Get-Date -Format yyyyMMdd).zip"
Invoke-WebRequest -Uri $binaryUrl -OutFile $binaryFile


# Unzip the binary package
$destinationPath = Join-Path -Path $downloadPath -ChildPath $repoName
Expand-Archive -Path $binaryFile -DestinationPath $destinationPath -Force

# Copy/deploy the binaries
$deployPath = Join-Path -Path $(Get-Location) -ChildPath $outputDir
Copy-Item -Path "$destinationPath\*" -Destination $deployPath -Recurse -Force


# If D3DDebug is set to true, download and deploy the debug package
if ($D3DDebug) {
    # Download the debug package
    $debugFile = Join-Path -Path $downloadPath -ChildPath "$($repoName)_debug_$(Get-Date -Format yyyyMMdd).zip"
    Invoke-WebRequest -Uri $debugUrl -OutFile $debugFile

    # Unzip the debug package
    $destinationDebugPath = Join-Path -Path $downloadPath -ChildPath "$($repoName)_debug"
    Expand-Archive -Path $debugFile -DestinationPath $destinationDebugPath -Force

    # Copy/deploy the debug files
    Copy-Item -Path "$destinationDebugPath\*" -Destination $deployPath -Recurse -Force
}