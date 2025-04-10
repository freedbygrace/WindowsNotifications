# WindowsNotifications PowerShell Module

This PowerShell module provides a convenient way to use the Windows Notifications library from PowerShell scripts.

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

### Initialize the Module

Before using the module, you need to initialize it:

```powershell
# Initialize with default settings
Initialize-WindowsNotifications

# Or initialize with a custom assembly path
Initialize-WindowsNotifications -AssemblyPath "C:\Path\To\WindowsNotifications.dll"

# Or initialize with a custom database path
Initialize-WindowsNotifications -DatabasePath "C:\Path\To\Notifications.db"
```

### Show Notifications

#### Simple Notification

```powershell
Show-Notification -Title "Hello" -Message "This is a simple notification"
```

#### Notification with Buttons

```powershell
$result = Show-Notification -Title "Action Required" -Message "Please select an option:" -Buttons "Approve", "Reject", "Defer"

if ($result.ClickedButtonText -eq "Approve") {
    Write-Host "User approved the action"
}
```

#### Reboot Notification with Deferrals

```powershell
$result = Show-Notification -Title "System Reboot Required" -Message "Your system needs to be rebooted." -RebootButtonText "Reboot Now" -DeferButtonText "Defer"

if ($result.ClickedButtonId -eq "reboot") {
    # Reboot the system
    Restart-Computer -Force
}
```

#### Advanced Options

```powershell
Show-Notification -Title "Advanced Notification" -Message "This notification has advanced options" `
    -Async -PersistState -TimeoutInSeconds 30 -LogoImagePath "C:\Path\To\Logo.png" `
    -HeroImagePath "C:\Path\To\Hero.png" -ShowReminder -ReminderTimeInMinutes 5
```

### Work with Notification Results

```powershell
# Get the result of a notification
$result = Get-NotificationResult -NotificationId "notification-id"

# Wait for a notification to complete
$result = Wait-Notification -NotificationId "notification-id" -TimeoutInSeconds 30

# Get all notification history
$history = Get-NotificationHistory

# Remove notification history
Remove-NotificationHistory -NotificationId "notification-id"

# Remove all notification history
Remove-NotificationHistory
```

### System Information

```powershell
# Check if running as SYSTEM
$isSystem = Test-SystemContext

# Get interactive user sessions
$sessions = Get-InteractiveUserSessions
```

## Command Reference

| Command | Description |
| ------- | ----------- |
| `Initialize-WindowsNotifications` | Initializes the Windows Notifications module |
| `Show-Notification` | Shows a notification with various options |
| `Get-NotificationResult` | Gets the result of a notification |
| `Wait-Notification` | Waits for a notification to complete |
| `Get-NotificationHistory` | Gets all notification results from the database |
| `Remove-NotificationHistory` | Deletes notification results from the database |
| `Get-InteractiveUserSessions` | Gets all interactive user sessions |
| `Test-SystemContext` | Checks if the current process is running as SYSTEM |

## Custom Branding

The module supports custom branding for notifications, allowing you to create notifications that match your organization's branding:

```powershell
Show-Notification -Title "IT Department Notification" -Message "System update required" `
    -BrandingText "Contoso IT" `
    -BrandingColor "#0078D7" `
    -AccentColor "#E81123" `
    -LogoImagePath "C:\Path\To\Logo.png" `
    -HeroImagePath "C:\Path\To\Banner.png" `
    -UseDarkTheme
```

### Supported Branding Options

| Option | Description |
| ------ | ----------- |
| `BrandingText` | Custom branding text (e.g., company or department name) |
| `BrandingColor` | Custom branding color (hex format: #RRGGBB) |
| `AccentColor` | Custom accent color for the notification (hex format: #RRGGBB) |
| `UseDarkTheme` | Whether to use dark theme for the notification |
| `LogoImagePath` | Path or URL to a logo image |
| `HeroImagePath` | Path or URL to a hero/banner image |
| `InlineImagePath` | Path or URL to an inline image |
| `AppIconPath` | Path or URL to an app icon |
| `BackgroundImagePath` | Path or URL to a background image |
| `AudioSource` | Audio source for the notification |
| `SilentMode` | Whether to show the notification in silent mode (no sound) |
| `DeadlineTime` | Deadline time for the notification |
| `ShowCountdown` | Whether to show a countdown timer on the notification |
| `DeadlineActionCommand` | Command to execute when the deadline is reached |
| `DeadlineActionArguments` | Arguments for the deadline command |
| `DeadlineActionUrl` | URL to open when the deadline is reached |
| `DeadlineActionScript` | PowerShell script to execute when the deadline is reached |
| `EnableLogging` | Whether to enable logging for the notification |
| `LogFilePath` | Path to the log file

## Examples

### Example 1: Simple Notification

```powershell
Import-Module WindowsNotifications
Initialize-WindowsNotifications

$result = Show-Notification -Title "Hello" -Message "This is a simple notification"
Write-Host "Notification displayed: $($result.Displayed)"
```

### Example 2: Notification with Buttons

```powershell
Import-Module WindowsNotifications
Initialize-WindowsNotifications

$result = Show-Notification -Title "Action Required" -Message "Please select an option:" -Buttons "Approve", "Reject", "Defer"

if ($result.ClickedButtonId) {
    Write-Host "Button clicked: $($result.ClickedButtonText)"

    switch ($result.ClickedButtonText) {
        "Approve" { Write-Host "User approved the action" }
        "Reject" { Write-Host "User rejected the action" }
        "Defer" { Write-Host "User deferred the action" }
    }
}
```

### Example 3: Asynchronous Notification

```powershell
Import-Module WindowsNotifications
Initialize-WindowsNotifications

$result = Show-Notification -Title "Background Task" -Message "A background task is running" -Async

# Do some work
for ($i = 1; $i -le 5; $i++) {
    Write-Host "Working... ($i/5)"
    Start-Sleep -Seconds 2

    # Check if the notification has been interacted with
    $currentResult = Get-NotificationResult -NotificationId $result.NotificationId

    if ($currentResult.Activated -or $currentResult.Dismissed) {
        Write-Host "User interacted with the notification"
        break
    }
}

Write-Host "Work completed"
```

### Example 4: Countdown and Deadline Notification

```powershell
Import-Module WindowsNotifications
Initialize-WindowsNotifications

# Create a notification with a deadline 5 minutes from now
$result = Show-Notification -Title "System Maintenance" -Message "Your system needs to restart soon." `
    -Buttons "Restart Now", "Defer" `
    -DeadlineTime (Get-Date).AddMinutes(5) `
    -ShowCountdown `
    -DeadlineActionScript "Restart-Computer -Force" `
    -EnableLogging `
    -LogFilePath "C:\Logs\notifications.log"

Write-Host "Notification displayed with deadline: $($result.Displayed)"
```
