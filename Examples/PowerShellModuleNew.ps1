# Example: PowerShell Module
# This script demonstrates how to use the PowerShell module

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
Initialize-WindowsNotifications

# Check if running as SYSTEM
$isSystem = Test-SystemContext
Write-Output "Running as SYSTEM: $isSystem"

# Get interactive user sessions
$sessions = Get-InteractiveUserSessions
Write-Output "Interactive user sessions: $($sessions.Count)"
foreach ($session in $sessions) {
    Write-Output "  - $session"
}

# Show a simple notification
$result = Show-Notification -Title "PowerShell Module" -Message "This notification was shown using the PowerShell module."

# Display the result
Write-Output "Notification displayed: $($result.Displayed)"
Write-Output "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Output "Notification was activated"
} elseif ($result.Dismissed) {
    Write-Output "Notification was dismissed"
}

# Get all notification results
$results = Get-AllNotificationResults
Write-Output "Total notification results: $($results.Count)"

# Clear all notification results
Clear-AllNotificationResults
Write-Output "All notification results cleared"
