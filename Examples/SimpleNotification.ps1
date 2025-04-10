# Example: Simple Notification
# This script demonstrates how to show a simple notification

# Load the WindowsNotifications assembly
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
if (Test-Path $dllPath) {
    $bytes = [System.IO.File]::ReadAllBytes($dllPath)
    $assembly = [System.Reflection.Assembly]::Load($bytes)
}
else {
    Write-Error "WindowsNotifications.dll not found at $dllPath. Please build the solution first."
    exit
}

# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Check if running as SYSTEM
$isSystem = $notificationManager.IsRunningAsSystem()
Write-Host "Running as SYSTEM: $isSystem"

# Get interactive user sessions
$sessions = $notificationManager.GetInteractiveUserSessions()
Write-Host "Interactive user sessions found: $($sessions.Count)"
foreach ($session in $sessions) {
    Write-Host "  $session"
}

# Show a simple notification
Write-Host "Showing simple notification..."
$result = $notificationManager.ShowSimpleNotification("Simple Notification", "This is a simple notification from PowerShell")

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was displayed, wait for interaction
if ($result.Displayed) {
    Write-Host "Waiting for user interaction..."
    $result = $notificationManager.WaitForNotification($result.NotificationId, 30000)
    
    if ($result -ne $null) {
        Write-Host "Notification was activated: $($result.Activated)"
        Write-Host "Notification was dismissed: $($result.Dismissed)"
    }
    else {
        Write-Host "Timed out waiting for user interaction"
    }
}
