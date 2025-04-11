# Example: Countdown and Deadline Notification
# This script demonstrates how to show a notification with a countdown timer and deadline

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
Initialize-WindowsNotifications -DllPath $dllPath

# Set a deadline 2 minutes from now
$deadline = (Get-Date).AddMinutes(2)

# Show a countdown notification
$result = Show-Notification -Title "System Maintenance Required" -Message "Your system needs to restart for maintenance. Please save your work." -DeadlineTime $deadline -ShowCountdown -Buttons "Restart Now", "Remind Me Later"

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Host "Notification was activated"
    if ($result.ClickedButtonId) {
        Write-Host "Button clicked: $($result.ClickedButtonText) (ID: $($result.ClickedButtonId))"
        
        # Handle button clicks
        if ($result.ClickedButtonText -eq "Restart Now") {
            Write-Host "User chose to restart now (simulated)"
            # In a real script, you would restart the computer here
            # Restart-Computer -Force
        } elseif ($result.ClickedButtonText -eq "Remind Me Later") {
            Write-Host "User chose to be reminded later"
            # In a real script, you would schedule a reminder here
        }
    }
} elseif ($result.Dismissed) {
    Write-Host "Notification was dismissed"
} elseif ($result.DeadlineReached) {
    Write-Host "Deadline was reached at $($result.DeadlineReachedTime)"
    Write-Host "Taking automatic action (simulated)"
    # In a real script, you would take the automatic action here
}
