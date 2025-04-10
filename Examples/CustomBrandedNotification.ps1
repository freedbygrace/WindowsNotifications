# Example: Custom Branded Notification
# This script demonstrates how to create a notification with custom branding

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

# Create custom notification options with branding
$options = New-Object WindowsNotifications.Models.NotificationOptions

# Set basic notification properties
$options.Title = "IT Department Notification"
$options.Message = "Your system has been selected for a security update. Please review the details below."

# Set branding properties
$options.BrandingText = "Contoso IT Department"
$options.BrandingColor = "#0078D7"  # Blue
$options.AccentColor = "#E81123"    # Red
$options.UseDarkTheme = $true

# Set custom images
# Note: Replace these paths with actual image paths on your system
$logoPath = Join-Path -Path $PSScriptRoot -ChildPath "Images\company-logo.png"
if (Test-Path $logoPath) {
    $options.LogoImagePath = $logoPath
}
else {
    # Use a URL as fallback
    $options.LogoImagePath = "https://www.contoso.com/images/logo.png"
}

$heroPath = Join-Path -Path $PSScriptRoot -ChildPath "Images\banner.png"
if (Test-Path $heroPath) {
    $options.HeroImagePath = $heroPath
}

# Add custom buttons with images
$updateButton = New-Object WindowsNotifications.Models.NotificationButton("Install Update", "install")
$updateButton.BackgroundColor = "#107C10"  # Green
$updateButton.TextColor = "#FFFFFF"        # White

$deferButton = New-Object WindowsNotifications.Models.NotificationButton("Defer", "defer")
$deferButton.BackgroundColor = "#5A5A5A"   # Gray
$deferButton.TextColor = "#FFFFFF"         # White

$moreInfoButton = New-Object WindowsNotifications.Models.NotificationButton("More Info", "info")
$moreInfoButton.IsContextMenu = $true

# Add buttons to the notification
$options.Buttons.Add($updateButton)
$options.Buttons.Add($deferButton)
$options.Buttons.Add($moreInfoButton)

# Configure audio
$options.AudioSource = "ms-winsoundevent:Notification.Default"
$options.SilentMode = $false

# Show the notification
Write-Host "Showing custom branded notification..."
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
        
        if ($result.ClickedButtonId) {
            Write-Host "Button clicked: $($result.ClickedButtonText) (ID: $($result.ClickedButtonId))"
            
            # Take action based on which button was clicked
            switch ($result.ClickedButtonId) {
                "install" {
                    Write-Host "User chose to install the update"
                    # In a real script, you would initiate the update here
                }
                "defer" {
                    Write-Host "User chose to defer the update"
                    # In a real script, you would schedule a reminder here
                }
                "info" {
                    Write-Host "User requested more information"
                    # In a real script, you would open a help page or display more details
                }
            }
        }
    }
}
