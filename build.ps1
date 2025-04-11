#
# build.ps1
# Build script for the Windows Notifications library
#

param (
    [switch]$Clean,
    [switch]$Release,
    [switch]$Test
)

# Set the configuration
$configuration = if ($Release) { "Release" } else { "Debug" }

# Set the MSBuild path
$msbuildPath = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
if (-not (Test-Path $msbuildPath)) {
    Write-Error "MSBuild not found at $msbuildPath"
    exit 1
}

# Clean the solution if requested
if ($Clean) {
    Write-Host "Cleaning solution..."
    & $msbuildPath "WindowsNotifications.sln" /t:Clean /p:Configuration=$configuration /v:minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to clean solution"
        exit 1
    }
}

# Build the solution
Write-Host "Building solution in $configuration configuration..."
& $msbuildPath "WindowsNotifications.sln" /t:Build /p:Configuration=$configuration /v:minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build solution"
    exit 1
}

# Run tests if requested
if ($Test) {
    Write-Host "Running tests..."
    $testDll = "WindowsNotifications.Tests\bin\$configuration\WindowsNotifications.Tests.dll"
    if (-not (Test-Path $testDll)) {
        Write-Error "Test DLL not found at $testDll"
        exit 1
    }
    
    # Try to find NUnit console runner
    $nunitPath = "packages\NUnit.ConsoleRunner\tools\nunit3-console.exe"
    if (-not (Test-Path $nunitPath)) {
        Write-Host "NUnit console runner not found, installing..."
        & nuget install NUnit.ConsoleRunner -OutputDirectory packages
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to install NUnit console runner"
            exit 1
        }
        
        $nunitPath = (Get-ChildItem -Path "packages" -Filter "nunit3-console.exe" -Recurse).FullName
        if (-not $nunitPath) {
            Write-Error "NUnit console runner not found after installation"
            exit 1
        }
    }
    
    & $nunitPath $testDll
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed"
        exit 1
    }
}

# Copy the DLL to the PowerShell module directory
Write-Host "Copying DLL to PowerShell module directory..."
$dllPath = "WindowsNotifications\bin\$configuration\WindowsNotifications.dll"
$modulePath = "PowerShell\WindowsNotifications.dll"
Copy-Item -Path $dllPath -Destination $modulePath -Force

Write-Host "Build completed successfully"
