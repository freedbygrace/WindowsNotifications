# Example: Load from Base64
# This script demonstrates how to load the WindowsNotifications assembly from a Base64 string

# First, convert the DLL to a Base64 string
# In a real scenario, you would have this string pre-generated
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
$bytes = [System.IO.File]::ReadAllBytes($dllPath)
$base64 = [Convert]::ToBase64String($bytes)

# Now, load the assembly from the Base64 string
$assemblyBytes = [Convert]::FromBase64String($base64)
$assembly = [System.Reflection.Assembly]::Load($assemblyBytes)

# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Show a simple notification
$result = $notificationManager.ShowSimpleNotification("Loaded from Base64", "This notification was shown using an assembly loaded from a Base64 string.")

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# If the notification was interacted with, show the details
if ($result.Activated) {
    Write-Host "Notification was activated"
} elseif ($result.Dismissed) {
    Write-Host "Notification was dismissed"
}
