# Example: PowerShell Module
# This script demonstrates how to use the PowerShell module

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
Initialize-WindowsNotifications -DllPath $dllPath

# Check if running as SYSTEM
$isSystem = Test-SystemContext
Write-Host "Running as SYSTEM: $isSystem"

# Get interactive user sessions
$sessions = Get-InteractiveUserSessions
Write-Host "Interactive user sessions: $($sessions.Count)"
foreach ($session in $sessions) {
    Write-Host "  - $session"
}

# Show a simple notification
$result = Show-Notification -Title "PowerShell Module" -Message "This notification was shown using the PowerShell module."

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Host "Notification was activated"
} elseif ($result.Dismissed) {
    Write-Host "Notification was dismissed"
}

# Get all notification results
$results = Get-AllNotificationResults
Write-Host "Total notification results: $($results.Count)"

# Clear all notification results
Clear-AllNotificationResults
Write-Host "All notification results cleared"
