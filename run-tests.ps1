# Run tests for SimpleWindowsNotifications

# Set the configuration
$Configuration = "Release"

# Check if dotnet CLI is available
$dotnetPath = Get-Command dotnet -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source
if (-not $dotnetPath) {
    Write-Host "The .NET SDK is not installed or not in the PATH. Please install the .NET SDK." -ForegroundColor Red
    exit 1
}

# Build the solution
Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build SimpleWindowsNotifications.sln --configuration $Configuration

# Check if the build was successful
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Run the tests
Write-Host "Running tests..." -ForegroundColor Cyan
dotnet test SimpleWindowsNotifications.sln --configuration $Configuration --no-build

# Check if the tests were successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "All tests passed!" -ForegroundColor Green
} else {
    Write-Host "Tests failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}
