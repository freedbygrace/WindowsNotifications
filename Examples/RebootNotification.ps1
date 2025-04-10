# Example: Reboot Notification with Deferrals
# This script demonstrates how to show a reboot notification with deferral options

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

# Create custom notification options for a reboot
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "System Reboot Required"
$options.Message = "Your system needs to be rebooted to complete important updates. Please save your work."
$options.PersistState = $true

# Add reboot button
$rebootButton = New-Object WindowsNotifications.Models.NotificationButton("Reboot Now", "reboot")
$options.Buttons.Add($rebootButton)

# Configure deferral options
$options.DeferralOptions = New-Object WindowsNotifications.Models.DeferralOptions
$options.DeferralOptions.DeferButtonText = "Defer Reboot"
$options.DeferralOptions.DeferralPrompt = "Postpone until:"
$options.DeferralOptions.MaxDeferrals = 3

# Clear existing deferral choices and add custom ones
$options.DeferralOptions.DeferralChoices.Clear()
$options.DeferralOptions.DeferralChoices.Add((New-Object WindowsNotifications.Models.DeferralOption("1 hour", [TimeSpan]::FromHours(1), "1hour")))
$options.DeferralOptions.DeferralChoices.Add((New-Object WindowsNotifications.Models.DeferralOption("4 hours", [TimeSpan]::FromHours(4), "4hours")))
$options.DeferralOptions.DeferralChoices.Add((New-Object WindowsNotifications.Models.DeferralOption("End of day", [TimeSpan]::FromHours(8), "endofday")))
$options.DeferralOptions.DeferralChoices.Add((New-Object WindowsNotifications.Models.DeferralOption("Tomorrow", [TimeSpan]::FromDays(1), "tomorrow")))

# Show the notification
Write-Host "Showing reboot notification..."
$result = $notificationManager.ShowNotification($options)

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
        
        if ($result.ClickedButtonId -eq "reboot") {
            Write-Host "User chose to reboot now"
            # In a real script, you would initiate a reboot here
            # Restart-Computer -Force
        }
        elseif ($result.Deferred) {
            Write-Host "User deferred the reboot until: $($result.DeferredUntil)"
            Write-Host "Deferral reason: $($result.DeferralReason)"
        }
    }
}
