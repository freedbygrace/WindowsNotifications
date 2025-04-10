# Example: Countdown and Deadline Notification
# This script demonstrates how to create a notification with a countdown timer and deadline action

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

# Create custom notification options
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "System Maintenance Required"
$options.Message = "Your system needs to restart for maintenance. Please save your work."
$options.PersistState = $true
$options.EnableLogging = $true

# Set up logging
$logFile = Join-Path -Path $PSScriptRoot -ChildPath "notification_log.txt"
$options.LogAction = {
    param($logEntry)
    Add-Content -Path $logFile -Value $logEntry
    Write-Host $logEntry
}

# Set deadline (5 minutes from now)
$options.DeadlineTime = (Get-Date).AddMinutes(5)
$options.ShowCountdown = $true

# Create deadline action (restart computer)
$deadlineAction = [WindowsNotifications.Models.DeadlineAction]::ExecuteScript("Write-Host 'System would restart now (simulated)'; Start-Sleep -Seconds 5")
$options.DeadlineAction = $deadlineAction

# Add buttons
$restartButton = New-Object WindowsNotifications.Models.NotificationButton("Restart Now", "restart")
$deferButton = New-Object WindowsNotifications.Models.NotificationButton("Defer", "defer")
$options.Buttons.Add($restartButton)
$options.Buttons.Add($deferButton)

# Configure deferral options
$options.DeferralOptions = New-Object WindowsNotifications.Models.DeferralOptions
$options.DeferralOptions.DeferButtonText = "Defer Restart"
$options.DeferralOptions.DeferralPrompt = "Postpone until:"
$options.DeferralOptions.MaxDeferrals = 2
$options.DeferralOptions.EnforceMaxDeferrals = $true
$options.DeferralOptions.ScheduleReminder = $true

# Clear existing deferral choices and add custom ones
$options.DeferralOptions.DeferralChoices.Clear()
$options.DeferralOptions.DeferralChoices.Add((New-Object WindowsNotifications.Models.DeferralOption("1 minute", [TimeSpan]::FromMinutes(1), "1min")))
$options.DeferralOptions.DeferralChoices.Add((New-Object WindowsNotifications.Models.DeferralOption("2 minutes", [TimeSpan]::FromMinutes(2), "2min")))
$options.DeferralOptions.DeferralChoices.Add((New-Object WindowsNotifications.Models.DeferralOption("3 minutes", [TimeSpan]::FromMinutes(3), "3min")))

# Set up event handlers
$options.OnActivated = {
    param($result)
    $message = "Notification was activated with button: $($result.ClickedButtonText)"
    Write-Host $message
    Add-Content -Path $logFile -Value $message
    
    if ($result.ClickedButtonId -eq "restart") {
        Write-Host "User chose to restart now (simulated)"
        # In a real script, you would restart the computer here
        # Restart-Computer -Force
    }
}

$options.OnTimeout = {
    param($result)
    $message = "Notification timed out"
    Write-Host $message
    Add-Content -Path $logFile -Value $message
}

$options.OnError = {
    param($result)
    $message = "Error occurred: $($result.ErrorMessage)"
    Write-Host $message -ForegroundColor Red
    Add-Content -Path $logFile -Value $message
}

# Show the notification
Write-Host "Showing countdown notification with deadline..."
$result = $notificationManager.ShowNotification($options)

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# Wait for the deadline to pass
Write-Host "Waiting for user interaction or deadline to pass..."
$waitTime = [int]([Math]::Ceiling(($options.DeadlineTime - (Get-Date)).TotalSeconds)) + 5
Start-Sleep -Seconds $waitTime

# Get the final result
$finalResult = $notificationManager.GetNotificationResult($result.NotificationId)
if ($finalResult -ne $null) {
    Write-Host "`nFinal notification state:"
    Write-Host "  Activated: $($finalResult.Activated)"
    Write-Host "  Dismissed: $($finalResult.Dismissed)"
    Write-Host "  Deferred: $($finalResult.Deferred)"
    
    if ($finalResult.Deferred) {
        Write-Host "  Deferred until: $($finalResult.DeferredUntil)"
        Write-Host "  Deferral reason: $($finalResult.DeferralReason)"
    }
    
    if ($finalResult.DeadlineReached) {
        Write-Host "  Deadline reached at: $($finalResult.DeadlineReachedTime)"
        Write-Host "  Deadline action: $($finalResult.DeadlineAction)"
    }
    
    if ($finalResult.ClickedButtonId) {
        Write-Host "  Button clicked: $($finalResult.ClickedButtonText)"
    }
}

Write-Host "`nLog file: $logFile"
