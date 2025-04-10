# Test script for SimpleWindowsNotifications

# Load the WindowsNotifications assembly
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "WindowsNotifications\bin\Release\WindowsNotifications.dll"
if (Test-Path $dllPath) {
    $bytes = [System.IO.File]::ReadAllBytes($dllPath)
    $assembly = [System.Reflection.Assembly]::Load($bytes)
    Write-Host "Assembly loaded: $($assembly.FullName)" -ForegroundColor Green
} else {
    Write-Error "WindowsNotifications.dll not found at $dllPath. Please build the solution first."
    exit
}

# Create a notification manager
$notificationManager = New-Object WindowsNotifications.SimpleNotificationManager
Write-Host "NotificationManager created" -ForegroundColor Green

# Show a simple notification
Write-Host "Showing a simple notification..." -ForegroundColor Cyan
$result = $notificationManager.ShowSimpleNotification("Hello, World!", "This is a test notification from the SimpleWindowsNotifications library.")
Write-Host "Notification displayed: $($result.Displayed)" -ForegroundColor Yellow
Write-Host "Notification ID: $($result.NotificationId)" -ForegroundColor Yellow

# Show a notification with buttons
Write-Host "`nShowing a notification with buttons..." -ForegroundColor Cyan
$result = $notificationManager.ShowNotificationWithButtons("Notification with Buttons", "Please select an option:", "OK", "Cancel")
Write-Host "Notification displayed: $($result.Displayed)" -ForegroundColor Yellow
Write-Host "Button clicked: $($result.ClickedButtonText)" -ForegroundColor Yellow

# Show a custom notification
Write-Host "`nShowing a custom notification..." -ForegroundColor Cyan
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "Custom Notification"
$options.Message = "This is a custom notification with options."
$options.Async = $true
$options.EnableLogging = $true
$options.LogAction = { param($logEntry) Write-Host $logEntry -ForegroundColor Gray }

$button1 = New-Object WindowsNotifications.Models.NotificationButton("Yes", "yes", "yes-arg")
$button2 = New-Object WindowsNotifications.Models.NotificationButton("No", "no", "no-arg")
$button3 = New-Object WindowsNotifications.Models.NotificationButton("Maybe", "maybe", "maybe-arg")
$options.Buttons.Add($button1)
$options.Buttons.Add($button2)
$options.Buttons.Add($button3)

$result = $notificationManager.ShowNotification($options)
Write-Host "Notification displayed: $($result.Displayed)" -ForegroundColor Yellow
Write-Host "Notification ID: $($result.NotificationId)" -ForegroundColor Yellow

Write-Host "`nTest completed successfully!" -ForegroundColor Green
