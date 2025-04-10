# Windows Notifications

A .NET DLL library for PowerShell 5 that displays notifications in user context from SYSTEM, with customization options, deferral support, LiteDB integration, and both synchronous/asynchronous operation modes.

## Features

- **User Context Notifications**: Display notifications in the user context from SYSTEM using user impersonation
- **Interactive Session Detection**: Only show notifications when an interactive user session (console or RDP) is present
- **Customizable Notifications**: Create simple or complex notifications with various customization options
- **Custom Branding**: Support for custom branding with logos, images, colors, and themes
- **Deferral Support**: Allow users to defer notifications (e.g., for system reboots)
- **State Persistence**: Save notification state using embedded LiteDB
- **PowerShell Integration**: Easily load and use the library in PowerShell 5
- **Synchronous/Asynchronous Modes**: Run notifications in blocking or non-blocking mode
- **Countdown Display**: Show countdown timers for time-sensitive notifications
- **Deadline Actions**: Configure custom actions to execute when notification deadlines are reached
- **Logging**: Comprehensive logging of notification events and user interactions

## Requirements

- Windows 10 or later
- .NET Framework 4.7.2 or later
- PowerShell 5.0 or later

## Installation

1. Download the latest release from the [Releases](https://github.com/freedbygrace/WindowsNotifications/releases) page
2. Extract the ZIP file to a location of your choice
3. Load the assembly in your PowerShell script using one of the methods described below

## Usage

### Loading the Assembly

There are several ways to load the WindowsNotifications assembly in PowerShell:

#### Method 1: Load from file

```powershell
$dllPath = "C:\Path\To\WindowsNotifications.dll"
$bytes = [System.IO.File]::ReadAllBytes($dllPath)
$assembly = [System.Reflection.Assembly]::Load($bytes)
```

#### Method 2: Load from Base64 string

```powershell
$base64 = "YOUR_BASE64_STRING_HERE"  # Replace with the actual Base64 string of the DLL
$bytes = [Convert]::FromBase64String($base64)
$assembly = [System.Reflection.Assembly]::Load($bytes)
```

### Basic Examples

#### Simple Notification

```powershell
# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Show a simple notification
$result = $notificationManager.ShowSimpleNotification("Title", "Message")
```

#### Notification with Buttons

```powershell
# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Show a notification with buttons
$result = $notificationManager.ShowNotificationWithButtons(
    "Action Required",
    "Please select an option below:",
    "Approve",
    "Reject",
    "Defer"
)

# Check which button was clicked
if ($result.ClickedButtonId) {
    Write-Host "Button clicked: $($result.ClickedButtonText)"
}
```

#### Reboot Notification with Deferrals

```powershell
# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Show a reboot notification
$result = $notificationManager.ShowRebootNotification(
    "System Reboot Required",
    "Your system needs to be rebooted to complete updates."
)

# Check the result
if ($result.ClickedButtonId -eq "reboot") {
    # Reboot the system
    Restart-Computer -Force
}
elseif ($result.Deferred) {
    Write-Host "Reboot deferred until: $($result.DeferredUntil)"
}
```

### Advanced Usage

#### Custom Notification Options

```powershell
# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Create custom notification options
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "Custom Notification"
$options.Message = "This is a custom notification"
$options.TimeoutInSeconds = 30
$options.Async = $true
$options.PersistState = $true

# Add buttons
$button1 = New-Object WindowsNotifications.Models.NotificationButton("OK", "ok")
$button2 = New-Object WindowsNotifications.Models.NotificationButton("Cancel", "cancel")
$options.Buttons.Add($button1)
$options.Buttons.Add($button2)

# Show the notification
$result = $notificationManager.ShowNotification($options)
```

#### Asynchronous Notifications

```powershell
# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Create notification options with async mode
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "Background Task"
$options.Message = "A background task is running"
$options.Async = $true

# Show the notification
$result = $notificationManager.ShowNotification($options)

# Do some work
# ...

# Check if the notification has been interacted with
$currentResult = $notificationManager.GetNotificationResult($result.NotificationId)
if ($currentResult.Activated) {
    Write-Host "User clicked the notification"
}
```

#### Custom Database Location

```powershell
# Create a notification manager with a custom database path
$dbPath = "C:\CustomPath\Notifications.db"
$notificationManager = New-Object WindowsNotifications.NotificationManager($dbPath)

# Show a notification
$result = $notificationManager.ShowSimpleNotification("Title", "Message")
```

#### Custom Branded Notifications

```powershell
# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Create custom notification options with branding
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "IT Department Notification"
$options.Message = "Your system has been selected for a security update."

# Set branding properties
$options.BrandingText = "Contoso IT Department"
$options.BrandingColor = "#0078D7"  # Blue
$options.AccentColor = "#E81123"    # Red
$options.UseDarkTheme = $true

# Set custom images (file paths or URLs)
$options.LogoImagePath = "C:\Path\To\Logo.png"  # Or "https://example.com/logo.png"
$options.HeroImagePath = "C:\Path\To\Banner.png"
$options.AppIconPath = "C:\Path\To\Icon.png"

# Add custom buttons with styling
$updateButton = New-Object WindowsNotifications.Models.NotificationButton("Install Update", "install")
$updateButton.BackgroundColor = "#107C10"  # Green
$updateButton.TextColor = "#FFFFFF"        # White
$options.Buttons.Add($updateButton)

# Show the notification
$result = $notificationManager.ShowNotification($options)
```

#### Countdown and Deadline Notifications

```powershell
# Create a notification manager
$notificationManager = New-Object WindowsNotifications.NotificationManager

# Create custom notification options
$options = New-Object WindowsNotifications.Models.NotificationOptions
$options.Title = "System Maintenance Required"
$options.Message = "Your system needs to restart for maintenance. Please save your work."

# Set deadline (5 minutes from now)
$options.DeadlineTime = (Get-Date).AddMinutes(5)
$options.ShowCountdown = $true

# Create deadline action (restart computer)
$deadlineAction = [WindowsNotifications.Models.DeadlineAction]::ExecuteScript(
    "Write-Host 'System would restart now'; Start-Sleep -Seconds 5"
)
$options.DeadlineAction = $deadlineAction

# Enable logging
$options.EnableLogging = $true
$options.LogAction = {
    param($logEntry)
    Add-Content -Path "C:\Logs\notifications.log" -Value $logEntry
    Write-Host $logEntry
}

# Show the notification
$result = $notificationManager.ShowNotification($options)
```

## API Reference

### NotificationManager Class

The main entry point for the Windows Notifications library.

#### Constructors

- `NotificationManager()` - Creates a new NotificationManager with the default database path
- `NotificationManager(string databasePath)` - Creates a new NotificationManager with the specified database path

#### Methods

- `NotificationResult ShowNotification(NotificationOptions options)` - Shows a notification with the specified options
- `NotificationResult ShowSimpleNotification(string title, string message)` - Shows a simple notification with the specified title and message
- `NotificationResult ShowNotificationWithButtons(string title, string message, params string[] buttons)` - Shows a notification with buttons
- `NotificationResult ShowRebootNotification(string title, string message, string rebootButtonText = "Reboot Now", string deferButtonText = "Defer")` - Shows a reboot notification with deferral options
- `NotificationResult GetNotificationResult(string notificationId)` - Gets the result of a notification
- `NotificationResult WaitForNotification(string notificationId, int timeout = -1)` - Waits for a notification to complete
- `List<NotificationResult> GetAllNotificationResults()` - Gets all notification results from the database
- `bool DeleteNotificationResult(string notificationId)` - Deletes a notification result from the database
- `bool DeleteAllNotificationResults()` - Deletes all notification results from the database
- `string GetDatabaseFilePath()` - Gets the path to the database file
- `bool IsRunningAsSystem()` - Checks if the current process is running as SYSTEM
- `List<string> GetInteractiveUserSessions()` - Gets all interactive user sessions

### NotificationOptions Class

Options for configuring a notification.

#### Properties

- `string Title` - The title of the notification
- `string Message` - The main message body of the notification
- `string LogoImagePath` - Optional logo image path
- `string HeroImagePath` - Optional hero image path
- `string Attribution` - Optional attribution text
- `int TimeoutInSeconds` - Optional timeout in seconds (0 = no timeout)
- `List<NotificationButton> Buttons` - Optional list of buttons to display
- `bool Async` - Whether to run the notification asynchronously
- `string Id` - Optional unique identifier for the notification
- `string Tag` - Optional tag for grouping notifications
- `string Group` - Optional group name for grouping notifications
- `DeferralOptions DeferralOptions` - Optional deferral options
- `bool ShowReminder` - Whether to show a reminder if the notification is not interacted with
- `int ReminderTimeInMinutes` - Time in minutes after which to show a reminder
- `bool PersistState` - Whether to persist the notification state in the database
- `bool EnableLogging` - Whether to enable logging for this notification
- `Action<string> LogAction` - Optional action to handle logging
- `Action<NotificationResult> OnActivated` - Optional action to execute when the notification is activated
- `Action<NotificationResult> OnTimeout` - Optional action to execute when the notification times out
- `Action<NotificationResult> OnError` - Optional action to execute when an error occurs
- `DateTime? DeadlineTime` - Optional deadline time for the notification
- `DeadlineAction DeadlineAction` - The action to take when the deadline is reached
- `bool ShowCountdown` - Whether to show a countdown timer on the notification

### NotificationResult Class

Represents the result of a notification interaction.

#### Properties

- `string NotificationId` - The unique identifier of the notification
- `bool Displayed` - Whether the notification was successfully displayed
- `bool Activated` - Whether the notification was activated (clicked)
- `bool Dismissed` - Whether the notification was dismissed
- `string ClickedButtonId` - The ID of the button that was clicked, if any
- `string ClickedButtonText` - The text of the button that was clicked, if any
- `string ClickedButtonArgument` - The argument of the button that was clicked, if any
- `DateTime CreatedTime` - The time when the notification was created
- `DateTime? InteractionTime` - The time when the notification was interacted with, if any
- `string ErrorMessage` - Any error message that occurred during the notification process
- `string ErrorCode` - The error code, if an error occurred
- `bool Deferred` - Whether the notification was deferred
- `DateTime? DeferredUntil` - The time when the notification was deferred until, if applicable
- `string DeferralReason` - The reason for deferral, if applicable
- `string DismissalReason` - The reason for dismissal, if applicable
- `string SystemAction` - The system action that was taken (e.g., snooze, dismiss)
- `bool DeadlineReached` - Whether the deadline was reached
- `DateTime? DeadlineReachedTime` - The time when the deadline was reached, if applicable
- `string DeadlineAction` - The action that was taken when the deadline was reached, if applicable

## PowerShell Module

The library includes a PowerShell module that makes it easier to use the Windows Notifications functionality in your PowerShell scripts. The module provides cmdlets for showing notifications, checking results, and managing notification history.

### Installing the PowerShell Module

1. Copy the files from the `PowerShell` directory to one of the following locations:
   - `%UserProfile%\Documents\WindowsPowerShell\Modules\WindowsNotifications` (for the current user)
   - `%ProgramFiles%\WindowsPowerShell\Modules\WindowsNotifications` (for all users)

2. Make sure the `WindowsNotifications.dll` file is in the same directory as the module files.

3. Import the module:
   ```powershell
   Import-Module WindowsNotifications
   ```

### Using the PowerShell Module

```powershell
# Initialize the module
Initialize-WindowsNotifications

# Show a simple notification
Show-Notification -Title "Hello" -Message "This is a simple notification"

# Show a notification with buttons
$result = Show-Notification -Title "Action Required" -Message "Please select an option:" -Buttons "Approve", "Reject", "Defer"

# Show a reboot notification
$result = Show-Notification -Title "System Reboot Required" -Message "Your system needs to be rebooted." -RebootButtonText "Reboot Now" -DeferButtonText "Defer"
```

See the [PowerShell/README.md](PowerShell/README.md) file for more information about the PowerShell module.

## Examples

See the [Examples](Examples) directory for complete PowerShell script examples:

- [SimpleNotification.ps1](Examples/SimpleNotification.ps1) - Shows a simple notification
- [NotificationWithButtons.ps1](Examples/NotificationWithButtons.ps1) - Shows a notification with buttons
- [RebootNotification.ps1](Examples/RebootNotification.ps1) - Shows a reboot notification with deferral options
- [AsyncNotification.ps1](Examples/AsyncNotification.ps1) - Shows an asynchronous notification
- [LoadFromBase64.ps1](Examples/LoadFromBase64.ps1) - Demonstrates loading the assembly from a Base64 string
- [PowerShellModule.ps1](Examples/PowerShellModule.ps1) - Demonstrates using the PowerShell module
- [CustomBrandedNotification.ps1](Examples/CustomBrandedNotification.ps1) - Demonstrates custom branding options
- [CountdownAndDeadline.ps1](Examples/CountdownAndDeadline.ps1) - Demonstrates countdown timers and deadline actions

## Building from Source

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Build the solution in Release mode
5. The compiled DLL will be in the `WindowsNotifications\bin\Release` directory

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.