# Windows Notifications PowerShell Module

This PowerShell module provides a simple interface to the Windows Notifications library, allowing you to display notifications in user context from SYSTEM.

## Installation

1. Copy the module files to one of the following locations:
   - `%UserProfile%\Documents\WindowsPowerShell\Modules\WindowsNotifications` (for the current user)
   - `%ProgramFiles%\WindowsPowerShell\Modules\WindowsNotifications` (for all users)

2. Make sure the `WindowsNotifications.dll` file is in the same directory as the module files.

3. Import the module:
   ```powershell
   Import-Module WindowsNotifications
   ```

## Usage

### Initializing the Module

```powershell
# Initialize with default settings
Initialize-WindowsNotifications

# Initialize with custom database path
Initialize-WindowsNotifications -DatabasePath "C:\Path\To\Notifications.db"
```

### Showing Notifications

#### Simple Notification

```powershell
Show-Notification -Title "Hello" -Message "This is a simple notification"
```

#### Notification with Buttons

```powershell
$result = Show-Notification -Title "Action Required" -Message "Please select an option:" -Buttons "Approve", "Reject", "Defer"

# Check which button was clicked
if ($result.ClickedButtonId) {
    Write-Host "Button clicked: $($result.ClickedButtonText)"
}
```

#### Reboot Notification

```powershell
$result = Show-Notification -Title "System Reboot Required" -Message "Your system needs to be rebooted." -RebootButtonText "Reboot Now" -DeferButtonText "Defer"

# Check if the reboot button was clicked
if ($result.ClickedButtonId -eq "reboot") {
    # Reboot the system
    Restart-Computer -Force
}
```

#### Asynchronous Notification

```powershell
$result = Show-Notification -Title "Background Task" -Message "A background task is running..." -Async

# Do some work
# ...

# Check if the notification has been interacted with
$currentResult = Get-NotificationResult -NotificationId $result.NotificationId
if ($currentResult.Activated) {
    Write-Host "User clicked the notification"
}
```

#### Notification with Deadline

```powershell
$deadline = (Get-Date).AddMinutes(5)
$result = Show-Notification -Title "System Maintenance" -Message "Your system needs maintenance." -DeadlineTime $deadline -ShowCountdown

# Check if the deadline was reached
if ($result.DeadlineReached) {
    Write-Host "Deadline reached at $($result.DeadlineReachedTime)"
}
```

### Managing Notification Results

```powershell
# Get a specific notification result
$result = Get-NotificationResult -NotificationId "12345678-1234-1234-1234-123456789012"

# Wait for a notification to complete
$result = Wait-Notification -NotificationId "12345678-1234-1234-1234-123456789012" -TimeoutInSeconds 30

# Get all notification results
$results = Get-AllNotificationResults

# Remove a notification result
Remove-NotificationResult -NotificationId "12345678-1234-1234-1234-123456789012"

# Clear all notification results
Clear-AllNotificationResults
```

### Utility Functions

```powershell
# Check if running as SYSTEM
$isSystem = Test-SystemContext

# Get interactive user sessions
$sessions = Get-InteractiveUserSessions
```

## Functions

- `Initialize-WindowsNotifications` - Initializes the Windows Notifications module
- `Show-Notification` - Shows a notification
- `Get-NotificationResult` - Gets the result of a notification
- `Wait-Notification` - Waits for a notification to complete
- `Get-AllNotificationResults` - Gets all notification results
- `Remove-NotificationResult` - Removes a notification result
- `Clear-AllNotificationResults` - Clears all notification results
- `Test-SystemContext` - Checks if running as SYSTEM
- `Get-InteractiveUserSessions` - Gets interactive user sessions
