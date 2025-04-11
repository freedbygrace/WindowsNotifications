# Example: Reboot Notification
# This script demonstrates how to show a reboot notification with deferral options

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
Initialize-WindowsNotifications -DllPath $dllPath

# Show a reboot notification
$result = Show-Notification -Title "System Reboot Required" -Message "Your system needs to be rebooted to complete updates." -RebootButtonText "Reboot Now" -DeferButtonText "Defer"

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Host "Notification was activated"
    if ($result.ClickedButtonId) {
        Write-Host "Button clicked: $($result.ClickedButtonText) (ID: $($result.ClickedButtonId))"
        
        # Handle button clicks
        if ($result.ClickedButtonId -eq "reboot") {
            Write-Host "User chose to reboot now (simulated)"
            # In a real script, you would reboot the computer here
            # Restart-Computer -Force
        }
    }
} elseif ($result.Dismissed) {
    Write-Host "Notification was dismissed"
} elseif ($result.Deferred) {
    Write-Host "Notification was deferred until: $($result.DeferredUntil)"
}
