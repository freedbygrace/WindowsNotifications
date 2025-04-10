# Example: Using the WindowsNotifications PowerShell Module
# This script demonstrates how to use the WindowsNotifications PowerShell module

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell"
Import-Module $modulePath -Force

# Initialize the module
# Note: The DLL must be in the same directory as the module files
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
if (Test-Path $dllPath) {
    Write-Host "Initializing WindowsNotifications module with DLL at: $dllPath"
    Initialize-WindowsNotifications -AssemblyPath $dllPath
}
else {
    Write-Error "WindowsNotifications.dll not found at $dllPath. Please build the solution first."
    exit
}

# Check if running as SYSTEM
$isSystem = Test-SystemContext
Write-Host "Running as SYSTEM: $isSystem"

# Get interactive user sessions
$sessions = Get-InteractiveUserSessions
Write-Host "Interactive user sessions found: $($sessions.Count)"
foreach ($session in $sessions) {
    Write-Host "  $session"
}

# Show a simple notification
Write-Host "`nShowing simple notification..."
$result = Show-Notification -Title "Simple Notification" -Message "This is a simple notification from the PowerShell module"

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# Show a notification with buttons
Write-Host "`nShowing notification with buttons..."
$result = Show-Notification -Title "Action Required" -Message "Please select an option:" -Buttons "Approve", "Reject", "Defer"

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

if ($result.ClickedButtonId) {
    Write-Host "Button clicked: $($result.ClickedButtonText)"
    
    switch ($result.ClickedButtonText) {
        "Approve" { Write-Host "User approved the action" }
        "Reject" { Write-Host "User rejected the action" }
        "Defer" { Write-Host "User deferred the action" }
    }
}

# Show a reboot notification
Write-Host "`nShowing reboot notification..."
$result = Show-Notification -Title "System Reboot Required" -Message "Your system needs to be rebooted to complete updates." -RebootButtonText "Reboot Now" -DeferButtonText "Defer"

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

if ($result.ClickedButtonId -eq "reboot") {
    Write-Host "User chose to reboot now"
    # In a real script, you would initiate a reboot here
    # Restart-Computer -Force
}
elseif ($result.Deferred) {
    Write-Host "User deferred the reboot until: $($result.DeferredUntil)"
    Write-Host "Deferral reason: $($result.DeferralReason)"
}

# Show an asynchronous notification
Write-Host "`nShowing asynchronous notification..."
$result = Show-Notification -Title "Background Task" -Message "A background task is running" -Async -PersistState

# Display the result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# Simulate doing some work
Write-Host "Performing background work..."
for ($i = 1; $i -le 5; $i++) {
    Write-Host "  Working... ($i/5)"
    Start-Sleep -Seconds 2
    
    # Check if the notification has been interacted with
    $currentResult = Get-NotificationResult -NotificationId $result.NotificationId
    
    if ($currentResult -ne $null -and ($currentResult.Activated -or $currentResult.Dismissed)) {
        if ($currentResult.ClickedButtonId) {
            Write-Host "User clicked: $($currentResult.ClickedButtonText)"
        }
        else {
            Write-Host "User interacted with the notification"
        }
    }
}

Write-Host "Background work completed"

# Get notification history
Write-Host "`nGetting notification history..."
$history = Get-NotificationHistory
Write-Host "Found $($history.Count) notifications in history"

foreach ($item in $history) {
    Write-Host "  ID: $($item.NotificationId), Created: $($item.CreatedTime), Activated: $($item.Activated), Dismissed: $($item.Dismissed)"
}

# Clean up notification history
Write-Host "`nCleaning up notification history..."
$result = Remove-NotificationHistory
Write-Host "Notification history cleared: $result"
