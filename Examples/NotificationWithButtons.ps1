# Example: Notification with Buttons
# This script demonstrates how to show a notification with buttons

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

# Show a notification with buttons
Write-Host "Showing notification with buttons..."
$result = $notificationManager.ShowNotificationWithButtons(
    "Action Required", 
    "Please select an option below:",
    "Approve",
    "Reject",
    "Defer"
)

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was displayed, wait for interaction
if ($result.Displayed) {
    Write-Host "Waiting for user interaction..."
    $result = $notificationManager.WaitForNotification($result.NotificationId)
    
    if ($result -ne $null) {
        Write-Host "Notification was activated: $($result.Activated)"
        Write-Host "Notification was dismissed: $($result.Dismissed)"
        
        if ($result.ClickedButtonId) {
            Write-Host "Button clicked: $($result.ClickedButtonText) (ID: $($result.ClickedButtonId))"
            
            # Take action based on which button was clicked
            switch ($result.ClickedButtonText) {
                "Approve" {
                    Write-Host "User approved the action"
                }
                "Reject" {
                    Write-Host "User rejected the action"
                }
                "Defer" {
                    Write-Host "User deferred the action"
                }
            }
        }
    }
}
