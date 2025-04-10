# Build script for SimpleWindowsNotifications

# Set the configuration
$Configuration = "Release"

# Check if dotnet CLI is available
$dotnetPath = Get-Command dotnet -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source
if (-not $dotnetPath) {
    Write-Host "The .NET SDK is not installed or not in the PATH. Please install the .NET SDK." -ForegroundColor Red
    exit 1
}

# Clean the solution
Write-Host "Cleaning solution..." -ForegroundColor Cyan
dotnet clean SimpleWindowsNotifications.sln --configuration $Configuration

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore SimpleWindowsNotifications.sln

# Build the solution
Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build SimpleWindowsNotifications.sln --configuration $Configuration --no-restore

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
