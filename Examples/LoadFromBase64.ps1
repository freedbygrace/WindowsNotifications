# Example: Load Assembly from Base64
# This script demonstrates how to load the WindowsNotifications assembly from a Base64 string
# This is useful for embedding the assembly directly in a script

# In a real scenario, you would replace this with the actual Base64 string of the assembly
# For demonstration purposes, we'll load the DLL and convert it to Base64
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
if (Test-Path $dllPath) {
    $bytes = [System.IO.File]::ReadAllBytes($dllPath)
    $base64 = [Convert]::ToBase64String($bytes)
    
    Write-Host "Assembly loaded and converted to Base64"
    Write-Host "Base64 string length: $($base64.Length) characters"
    
    # In a real script, the base64 string would be hardcoded here
    # $base64 = "YOUR_BASE64_STRING_HERE"
    
    # Load the assembly from the Base64 string
    $assemblyBytes = [Convert]::FromBase64String($base64)
    $assembly = [System.Reflection.Assembly]::Load($assemblyBytes)
    
    Write-Host "Assembly loaded successfully: $($assembly.FullName)"
    
    # Create a notification manager and show a simple notification
    $notificationManager = New-Object WindowsNotifications.NotificationManager
    $result = $notificationManager.ShowSimpleNotification(
        "Loaded from Base64", 
        "This notification was shown from an assembly loaded from a Base64 string"
    )
    
    Write-Host "Notification displayed: $($result.Displayed)"
}
else {
    Write-Error "WindowsNotifications.dll not found at $dllPath. Please build the solution first."
}
