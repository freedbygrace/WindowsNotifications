# Windows Notifications

A .NET DLL library for PowerShell 5 or 7 that displays notifications in user context from SYSTEM, with customization options, deferral support, LiteDB integration, and both synchronous/asynchronous operation modes.

## Features

- **User Context Notifications**: Display notifications in the user context from SYSTEM using user impersonation
- **Interactive Session Detection**: Only show notifications when an interactive user session (console or RDP) is present
- **Customizable Notifications**: Create simple or complex notifications with various customization options
- **Custom Branding**: Support for custom branding with logos, images, and attribution
- **Deferral Support**: Allow users to defer notifications (e.g., for system reboots)
- **State Persistence**: Save notification state using embedded LiteDB
- **PowerShell Integration**: Easily load and use the library in PowerShell 5 or 7
- **Synchronous/Asynchronous Modes**: Run notifications in blocking or non-blocking mode
- **Countdown Display**: Show countdown timers for time-sensitive notifications
- **Deadline Actions**: Configure custom actions to execute when notification deadlines are reached

## Project Structure

- `WindowsNotifications/` - Core .NET library
- `WindowsNotifications.Tests/` - Unit tests
- `PowerShell/` - PowerShell module
- `Examples/` - Example scripts

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

### PowerShell Module

The library includes a PowerShell module that makes it easier to use the Windows Notifications functionality in your PowerShell scripts.

```powershell
# Import the module
Import-Module WindowsNotifications

# Initialize the module
Initialize-WindowsNotifications

# Show a simple notification
Show-Notification -Title "Hello" -Message "This is a simple notification"

# Show a notification with buttons
$result = Show-Notification -Title "Action Required" -Message "Please select an option:" -Buttons "Approve", "Reject", "Defer"
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
2. Run the build script: `./build.ps1 -Release`
3. The compiled DLL will be in the `WindowsNotifications\bin\Release` directory
4. The PowerShell module will be in the `PowerShell` directory

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.