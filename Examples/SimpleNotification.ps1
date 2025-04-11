# Example: Simple Notification
# This script demonstrates how to show a simple notification

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
Initialize-WindowsNotifications -DllPath $dllPath

# Show a simple notification
$result = Show-Notification -Title "Simple Notification" -Message "This is a simple notification." -TimeoutInSeconds 10

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Host "Notification was activated"
} elseif ($result.Dismissed) {
    Write-Host "Notification was dismissed"
}
