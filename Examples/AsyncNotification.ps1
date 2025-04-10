# Example: Asynchronous Notification
# This script demonstrates how to show an asynchronous notification and check its status later

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
$options.Title = "Background Task Running"
$options.Message = "A background task is running. You can continue working."
$options.Async = $true  # Run asynchronously
$options.PersistState = $true  # Save state to database

# Add buttons
$viewButton = New-Object WindowsNotifications.Models.NotificationButton("View Progress", "view")
$cancelButton = New-Object WindowsNotifications.Models.NotificationButton("Cancel Task", "cancel")
$options.Buttons.Add($viewButton)
$options.Buttons.Add($cancelButton)

# Show the notification
Write-Host "Showing asynchronous notification..."
$result = $notificationManager.ShowNotification($options)

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# Simulate doing some work
Write-Host "Performing background work..."
for ($i = 1; $i -le 5; $i++) {
    Write-Host "  Working... ($i/5)"
    Start-Sleep -Seconds 2
    
    # Check if the notification has been interacted with
    $currentResult = $notificationManager.GetNotificationResult($result.NotificationId)
    
    if ($currentResult -ne $null -and ($currentResult.Activated -or $currentResult.Dismissed)) {
        if ($currentResult.ClickedButtonId -eq "cancel") {
            Write-Host "User canceled the task!"
            break
        }
        elseif ($currentResult.ClickedButtonId -eq "view") {
            Write-Host "User viewed the progress!"
            
            # Show a new notification with updated progress
            $progressOptions = New-Object WindowsNotifications.Models.NotificationOptions
            $progressOptions.Title = "Task Progress"
            $progressOptions.Message = "Progress: $i/5 steps completed"
            $progressOptions.Tag = "progress"  # Group with the same tag
            $notificationManager.ShowNotification($progressOptions)
        }
    }
}

Write-Host "Background work completed"

# Show a completion notification
$completionOptions = New-Object WindowsNotifications.Models.NotificationOptions
$completionOptions.Title = "Task Completed"
$completionOptions.Message = "The background task has finished."
$completionOptions.Tag = "progress"  # Replace previous notification with same tag
$notificationManager.ShowNotification($completionOptions)

# Get the final notification result
$finalResult = $notificationManager.GetNotificationResult($result.NotificationId)
if ($finalResult -ne $null) {
    Write-Host "Final notification state:"
    Write-Host "  Activated: $($finalResult.Activated)"
    Write-Host "  Dismissed: $($finalResult.Dismissed)"
    if ($finalResult.ClickedButtonId) {
        Write-Host "  Button clicked: $($finalResult.ClickedButtonText)"
    }
}
