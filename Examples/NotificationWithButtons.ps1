# Example: Notification with Buttons
# This script demonstrates how to show a notification with buttons

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
Initialize-WindowsNotifications -DllPath $dllPath

# Show a notification with buttons
$result = Show-Notification -Title "Action Required" -Message "Please select an option:" -Buttons "Approve", "Reject", "Defer"

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Host "Notification was activated"
    if ($result.ClickedButtonId) {
        Write-Host "Button clicked: $($result.ClickedButtonText) (ID: $($result.ClickedButtonId))"
    }
} elseif ($result.Dismissed) {
    Write-Host "Notification was dismissed"
}
