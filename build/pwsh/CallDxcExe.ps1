param (
    [string]$shaderFile ,
    [string]$shaderType ,
    [string]$shaderProf ,
    [string]$entryPoint = "main",
    [string]$outputDir  = ".\dxc\build\",
    [string]$d3dDebug   = "false"
)
$dxcTool    = "dxc.exe"
$shaderFile = Resolve-Path $shaderFile
$shaderFile = $shaderFile.Path
$shaderFile = $shaderFile.Replace("\", "/")
# If shader file doesn't exist, throw an error:
if (-not (Test-Path $shaderFile)) {
    throw "Shader file '$shaderFile' does not exist!"
}
# Get the literal path to the output directory:
$outputDir  = Resolve-Path $outputDir

# Sanitize the shader type and profile strings:
$shaderType = $shaderType.ToLower()
$shaderType = $shaderType.Replace(" ", "")
$shaderProf = $shaderProf.ToLower()
$shaderProf = $shaderProf.Replace(" ", "")

# Create the output directory if it doesn't exist:
if ( -not (Test-Path $outputDir) ) {
    New-Item -ItemType Directory -Force -Path $outputDir
}

# Create the output file name:
$outputFile = $shaderFile
# Replace any extension with .cso:
$outputFile = $outputFile.Replace( $outputFile.Extension, ".cso" )
# Replace the shader file's directory with the output directory:
$outputFile = $outputFile.Replace( $outputFile.DirectoryName, $outputDir )
Write-Host "Output directory:  '$outputFile.DirectoryName' ..."
Write-Host "Output type:       '$outputFile.Extension' ..."
Write-Host "Output file:       '$outputFile.FileName' ..."

# Create the output directory if it doesn't exist:
if ( -not (Test-Path $outputFile.DirectoryName) ) {
    Write-Host "Output directory '$outputFile.DirectoryName' does not exist, creating it now ..."
    New-Item -ItemType Directory -Force -Path $outputFile.DirectoryName | Out-Null
}

# Check if DXC is installed and has been added to the path:
Write-Host "`nChecking for local Dxc.exe Shader Compiler tool installations ..."
checkIfDxcInstalled

# Invoke the compiler with the input file, output file, shader type, and profile:
# Example:  & $dxcTool -T $shaderType -E $entryPoint -Fo $outputFile -fspv-target-env=vulkan1.1 -fvk-use-dx-layout -spirv -fspv-extension=SPV_KHR_shader_ballot -fspv-extension=SPV_KHR_shader_draw_parameters -fspv-extension=SPV_KHR_subgroup_vote -fspv-extension=SPV_KHR_16bit_storage -fspv-extension=SPV_KHR_8bit_storage -fspv-extension=SPV_KHR_storage_buffer_storage_class -fspv-extension=SPV_KHR_variable_pointers -fspv-extension=SPV_KHR_shader_float16_int8 -fspv-extension=SPV_KHR_shader_float_controls -fspv-extension=SPV_KHR_image_gather_extended -fspv-extension=SPV_KHR_shader_image_gather_extended -fspv-extension=SPV_KHR_shader_image_read_write -fspv-extension=SPV_KHR_shader_non_semantic_info -fspv-extension=SPV_KHR_shader_subgroup_extended_types -fspv-extension=SPV_KHR_shader_terminate_invocation -fspv-extension=SPV_KHR_subgroup_ballot -fspv-extension=SPV_KHR_subgroup_extended_types -fspv-extension=SPV_KHR_subgroup_vote -fspv-extension=SPV_KHR_variable_pointers -fspv-extension=SPV_KHR_vulkan_memory_model -fspv-extension=SPV_KHR_workgroup_memory_explicit_layout -fspv-extension=SPV_EXT_descriptor_indexing -fspv-extension=SPV_EXT_shader_subgroup_ballot -fspv-extension=SPV_EXT_shader_subgroup_vote -fspv-extension=SPV_KHR_shader_ballot -fspv-extension=SPV_KHR_shader_draw_parameters -fspv-extension=SPV_KHR_subgroup_vote -fspv-extension=SPV_KHR_16bit_storage -fspv-extension=SPV_KHR_8bit_storage -fspv-extension=SPV_KHR_storage_buffer_storage_class -fspv-extension=SPV_KHR_variable_pointers -fspv-extension=SPV_KHR_shader_float16_int8 -fspv-extension=SPV_KHR_shader_float_controls -fspv-extension=SPV_KHR_image_gather_extended -fspv-extension=SPV_KHR_shader_image_gather_extended -fspv-extension=SPV_KHR_shader_image_read_write -fspv-extension=SPV_KHR_shader_non_semantic_info -fspv-extension=SPV_KHR_shader_subgroup_extended_types -fspv-extension=SP

Write-Host "`nPreparing shader compilation for: '$shaderFile' ..."
Write-Host "Input file:       '$shaderFile' ..."
Write-Host "Shader type:      '$shaderType' ..."
Write-Host "Shader profile:   '$shaderProf' ..."
Write-Host "Entry point:      '$entryPoint' ..."
Write-Host "Output file:      '$outputFile' ..."
Write-Host "D3D debug:        '$d3dDebug' ..."

# Save process result code in a variable:
$job = Start-Job -ScriptBlock {
    & $dxcTool -T $shaderType -E $entryPoint -Fo $outputFile
}
# Make sure the job has completed
$job | Wait-Job

# Receive the job output and display it:
$job | Receive-Job

# Get the job's exit code:
$jobExitCode = $job | Receive-Job -Wait -AutoRemoveJob

# Check if the job failed:
if ( $jobExitCode -ne 0 ) {
    Write-Host "(!) Failure: Shader compilation failed with exit code: '$jobExitCode'!"
    throw "Shader compilation failed with exit code '$jobExitCode'!"
}
# Check if the output file exists:
if ( -not (Test-Path $outputFile) ) {
    Write-Host "(!) Failure: Shader compilation failed, output file '$outputFile' does not exist!"
    throw "Shader compilation failed, output file '$outputFile' does not exist!"
}
# Check if the output file is empty:
if ( (Get-Item $outputFile).Length -eq 0 ) {
    Write-Host "(!) Failure: Shader compilation failed, output file '$outputFile' is empty!"
    throw "Shader compilation failed, output file '$outputFile' is empty!"
}


# Display the compiler std output:
Write-Host "`nShader compilation completed successfully!"
Write-Host "Compiler output:"
$job | Receive-Job -Wait -AutoRemoveJob


# Clean up the job
$job | Remove-Job

# -------------------------------------------------------------------------------------------------
# Checks if the dxc.exe tool is installed and 
# the environment variable for the tool exists:
function checkIfDxcInstalled {
    $dxcTool = "dxc.exe"
    if ( -not (Test-Path $dxcTool) -and -not (Test-Path $env:DXC) ) {
        Write-Host "(!) Failure: The tool 'dxc.exe' (DirectX Shader Compiler) is not installed!"
        throw "DXC environment variable '$env:DXC' does not exist!"
    }
}
# -------------------------------------------------------------------------------------------------