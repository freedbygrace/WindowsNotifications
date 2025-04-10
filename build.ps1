# Build script for WindowsNotifications

# Set the configuration
$Configuration = "Release"

# Find MSBuild
$msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
if (-not (Test-Path $msbuildPath)) {
    $msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
}
if (-not (Test-Path $msbuildPath)) {
    $msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
}
if (-not (Test-Path $msbuildPath)) {
    $msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
}
if (-not (Test-Path $msbuildPath)) {
    # Try to find MSBuild in the PATH
    $msbuildPath = "MSBuild.exe"
}

# Clean the solution
Write-Host "Cleaning solution..." -ForegroundColor Cyan
& $msbuildPath WindowsNotifications.sln /t:Clean /p:Configuration=$Configuration

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
& $msbuildPath WindowsNotifications.sln /t:Restore /p:Configuration=$Configuration

# Build the solution
Write-Host "Building solution..." -ForegroundColor Cyan
& $msbuildPath WindowsNotifications.sln /t:Build /p:Configuration=$Configuration

# Check if the build was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build completed successfully!" -ForegroundColor Green
    
    # Show the output directory
    $outputDir = Join-Path -Path $PSScriptRoot -ChildPath "WindowsNotifications\bin\$Configuration"
    Write-Host "Output directory: $outputDir" -ForegroundColor Yellow
    
    # List the files in the output directory
    Get-ChildItem -Path $outputDir | Format-Table Name, Length
} else {
    Write-Host "Build failed with exit code $LASTEXITCODE" -ForegroundColor Red
}
