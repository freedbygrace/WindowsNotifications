# Example: Custom Branded Notification
# This script demonstrates how to show a notification with custom branding

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
Initialize-WindowsNotifications -DllPath $dllPath

# Create custom notification options
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "IT Department Notification"
$options.Message = "Your system has been selected for a security update."

# Set branding properties
$options.Attribution = "Contoso IT Department"

# Set custom images (file paths or URLs)
$logoPath = Join-Path -Path $PSScriptRoot -ChildPath "logo.png"
if (Test-Path $logoPath) {
    $options.LogoImagePath = $logoPath
}

$heroPath = Join-Path -Path $PSScriptRoot -ChildPath "banner.png"
if (Test-Path $heroPath) {
    $options.HeroImagePath = $heroPath
}

# Add custom buttons
$updateButton = New-Object WindowsNotifications.Models.NotificationButton("Install Update", "install")
$laterButton = New-Object WindowsNotifications.Models.NotificationButton("Later", "later")
$options.Buttons.Add($updateButton)
$options.Buttons.Add($laterButton)

# Show the notification
$notificationManager = New-Object WindowsNotifications.NotificationManager
$result = $notificationManager.ShowNotification($options)

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Host "Notification was activated"
    if ($result.ClickedButtonId) {
        Write-Host "Button clicked: $($result.ClickedButtonText) (ID: $($result.ClickedButtonId))"
        
        # Handle button clicks
        if ($result.ClickedButtonId -eq "install") {
            Write-Host "User chose to install the update (simulated)"
            # In a real script, you would install the update here
        } elseif ($result.ClickedButtonId -eq "later") {
            Write-Host "User chose to install later"
            # In a real script, you would schedule a reminder here
        }
    }
} elseif ($result.Dismissed) {
    Write-Host "Notification was dismissed"
}
