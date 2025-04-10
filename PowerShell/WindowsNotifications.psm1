#
# WindowsNotifications.psm1
# PowerShell module for the Windows Notifications library
#

# Module variables
$script:NotificationAssembly = $null
$script:NotificationManager = $null
$script:DefaultDatabasePath = $null

#
# Private functions
#

function Initialize-NotificationAssembly {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$false)]
        [string]$AssemblyPath
    )

    try {
        # If assembly path is provided and exists, load from file
        if ($AssemblyPath -and (Test-Path $AssemblyPath)) {
            $bytes = [System.IO.File]::ReadAllBytes($AssemblyPath)
            $script:NotificationAssembly = [System.Reflection.Assembly]::Load($bytes)
        }
        # Otherwise, try to load from the module directory
        else {
            $moduleRoot = Split-Path -Parent $PSCommandPath
            $defaultPath = Join-Path -Path $moduleRoot -ChildPath "WindowsNotifications.dll"

            if (Test-Path $defaultPath) {
                $bytes = [System.IO.File]::ReadAllBytes($defaultPath)
                $script:NotificationAssembly = [System.Reflection.Assembly]::Load($bytes)
            }
            else {
                throw "WindowsNotifications.dll not found. Please specify the path using the -AssemblyPath parameter."
            }
        }

        # Create the notification manager
        $script:NotificationManager = New-Object WindowsNotifications.NotificationManager
        $script:DefaultDatabasePath = $script:NotificationManager.GetDatabaseFilePath()

        return $true
    }
    catch {
        Write-Error "Failed to initialize WindowsNotifications assembly: $_"
        return $false
    }
}

#
# Public functions
#

function Initialize-WindowsNotifications {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$false)]
        [string]$AssemblyPath,

        [Parameter(Mandatory=$false)]
        [string]$DatabasePath
    )

    # Initialize the assembly
    if (-not (Initialize-NotificationAssembly -AssemblyPath $AssemblyPath)) {
        return $false
    }

    # If a custom database path is specified, create a new notification manager with that path
    if ($DatabasePath) {
        try {
            $script:NotificationManager = New-Object WindowsNotifications.NotificationManager($DatabasePath)
            $script:DefaultDatabasePath = $script:NotificationManager.GetDatabaseFilePath()
        }
        catch {
            Write-Error "Failed to initialize NotificationManager with custom database path: $_"
            return $false
        }
    }

    Write-Host "WindowsNotifications initialized successfully."
    Write-Host "Database path: $script:DefaultDatabasePath"

    # Check if running as SYSTEM
    $isSystem = $script:NotificationManager.IsRunningAsSystem()
    Write-Host "Running as SYSTEM: $isSystem"

    # Get interactive user sessions
    $sessions = $script:NotificationManager.GetInteractiveUserSessions()
    Write-Host "Interactive user sessions found: $($sessions.Count)"

    return $true
}

function Show-Notification {
    [CmdletBinding(DefaultParameterSetName="Simple")]
    param (
        [Parameter(Mandatory=$true, ParameterSetName="Simple")]
        [Parameter(Mandatory=$true, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$true, ParameterSetName="Reboot")]
        [string]$Title,

        [Parameter(Mandatory=$true, ParameterSetName="Simple")]
        [Parameter(Mandatory=$true, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$true, ParameterSetName="Reboot")]
        [string]$Message,

        [Parameter(Mandatory=$true, ParameterSetName="WithButtons")]
        [string[]]$Buttons,

        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$RebootButtonText = "Reboot Now",

        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$DeferButtonText = "Defer",

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$Async,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$PersistState,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [int]$TimeoutInSeconds = 0,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$LogoImagePath,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$HeroImagePath,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$InlineImagePath,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$AppIconPath,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$BackgroundImagePath,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$BrandingText,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$BrandingColor,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$AccentColor,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$UseDarkTheme,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$AudioSource,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$SilentMode,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$ShowReminder,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [int]$ReminderTimeInMinutes = 60,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [DateTime]$DeadlineTime,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$ShowCountdown,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$DeadlineActionCommand,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$DeadlineActionArguments,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$DeadlineActionUrl,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$DeadlineActionScript,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$EnableLogging,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$LogFilePath
    )

    # Ensure the notification assembly is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        # Handle different parameter sets
        switch ($PSCmdlet.ParameterSetName) {
            "Simple" {
                # Create custom notification options for simple notification
                $options = New-Object WindowsNotifications.Models.NotificationOptions
                $options.Title = $Title
                $options.Message = $Message
                $options.Async = $Async
                $options.PersistState = $PersistState
                $options.TimeoutInSeconds = $TimeoutInSeconds
                $options.ShowReminder = $ShowReminder
                $options.ReminderTimeInMinutes = $ReminderTimeInMinutes

                # Set image paths
                if ($LogoImagePath) { $options.LogoImagePath = $LogoImagePath }
                if ($HeroImagePath) { $options.HeroImagePath = $HeroImagePath }
                if ($InlineImagePath) { $options.InlineImagePath = $InlineImagePath }
                if ($AppIconPath) { $options.AppIconPath = $AppIconPath }
                if ($BackgroundImagePath) { $options.BackgroundImagePath = $BackgroundImagePath }

                # Set branding options
                if ($BrandingText) { $options.BrandingText = $BrandingText }
                if ($BrandingColor) { $options.BrandingColor = $BrandingColor }
                if ($AccentColor) { $options.AccentColor = $AccentColor }
                if ($UseDarkTheme) { $options.UseDarkTheme = $UseDarkTheme }

                # Set audio options
                if ($AudioSource) { $options.AudioSource = $AudioSource }
                if ($SilentMode) { $options.SilentMode = $SilentMode }

                # Set deadline and countdown options
                if ($DeadlineTime -ne $null) { $options.DeadlineTime = $DeadlineTime }
                if ($ShowCountdown) { $options.ShowCountdown = $ShowCountdown }

                # Set deadline action
                if ($DeadlineActionCommand) {
                    $options.DeadlineAction = New-Object WindowsNotifications.Models.DeadlineAction($DeadlineActionCommand, $DeadlineActionArguments)
                }
                elseif ($DeadlineActionUrl) {
                    $options.DeadlineAction = [WindowsNotifications.Models.DeadlineAction]::OpenUrl($DeadlineActionUrl)
                }
                elseif ($DeadlineActionScript) {
                    $options.DeadlineAction = [WindowsNotifications.Models.DeadlineAction]::ExecuteScript($DeadlineActionScript)
                }

                # Set logging options
                if ($EnableLogging) { $options.EnableLogging = $true }
                if ($LogFilePath) {
                    $options.LogAction = {
                        param($logEntry)
                        Add-Content -Path $LogFilePath -Value $logEntry
                    }
                }

                $result = $script:NotificationManager.ShowNotification($options)
            }
            "WithButtons" {
                # Create custom notification options for notification with buttons
                $options = New-Object WindowsNotifications.Models.NotificationOptions
                $options.Title = $Title
                $options.Message = $Message
                $options.Async = $Async
                $options.PersistState = $PersistState
                $options.TimeoutInSeconds = $TimeoutInSeconds
                $options.ShowReminder = $ShowReminder
                $options.ReminderTimeInMinutes = $ReminderTimeInMinutes

                # Set image paths
                if ($LogoImagePath) { $options.LogoImagePath = $LogoImagePath }
                if ($HeroImagePath) { $options.HeroImagePath = $HeroImagePath }
                if ($InlineImagePath) { $options.InlineImagePath = $InlineImagePath }
                if ($AppIconPath) { $options.AppIconPath = $AppIconPath }
                if ($BackgroundImagePath) { $options.BackgroundImagePath = $BackgroundImagePath }

                # Set branding options
                if ($BrandingText) { $options.BrandingText = $BrandingText }
                if ($BrandingColor) { $options.BrandingColor = $BrandingColor }
                if ($AccentColor) { $options.AccentColor = $AccentColor }
                if ($UseDarkTheme) { $options.UseDarkTheme = $UseDarkTheme }

                # Set audio options
                if ($AudioSource) { $options.AudioSource = $AudioSource }
                if ($SilentMode) { $options.SilentMode = $SilentMode }

                # Set deadline and countdown options
                if ($DeadlineTime -ne $null) { $options.DeadlineTime = $DeadlineTime }
                if ($ShowCountdown) { $options.ShowCountdown = $ShowCountdown }

                # Set deadline action
                if ($DeadlineActionCommand) {
                    $options.DeadlineAction = New-Object WindowsNotifications.Models.DeadlineAction($DeadlineActionCommand, $DeadlineActionArguments)
                }
                elseif ($DeadlineActionUrl) {
                    $options.DeadlineAction = [WindowsNotifications.Models.DeadlineAction]::OpenUrl($DeadlineActionUrl)
                }
                elseif ($DeadlineActionScript) {
                    $options.DeadlineAction = [WindowsNotifications.Models.DeadlineAction]::ExecuteScript($DeadlineActionScript)
                }

                # Set logging options
                if ($EnableLogging) { $options.EnableLogging = $true }
                if ($LogFilePath) {
                    $options.LogAction = {
                        param($logEntry)
                        Add-Content -Path $LogFilePath -Value $logEntry
                    }
                }

                # Add buttons
                foreach ($buttonText in $Buttons) {
                    $button = New-Object WindowsNotifications.Models.NotificationButton($buttonText)
                    $options.Buttons.Add($button)
                }

                $result = $script:NotificationManager.ShowNotification($options)
            }
            "Reboot" {
                # Use the built-in reboot notification method
                $result = $script:NotificationManager.ShowRebootNotification($Title, $Message, $RebootButtonText, $DeferButtonText)
            }
        }

        return $result
    }
    catch {
        Write-Error "Failed to show notification: $_"
        return $null
    }
}

function Get-NotificationResult {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string]$NotificationId
    )

    # Ensure the notification assembly is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        return $script:NotificationManager.GetNotificationResult($NotificationId)
    }
    catch {
        Write-Error "Failed to get notification result: $_"
        return $null
    }
}

function Wait-Notification {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string]$NotificationId,

        [Parameter(Mandatory=$false)]
        [int]$TimeoutInSeconds = -1
    )

    # Ensure the notification assembly is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        $timeoutMs = $TimeoutInSeconds -eq -1 ? -1 : ($TimeoutInSeconds * 1000)
        return $script:NotificationManager.WaitForNotification($NotificationId, $timeoutMs)
    }
    catch {
        Write-Error "Failed to wait for notification: $_"
        return $null
    }
}

function Get-NotificationHistory {
    [CmdletBinding()]
    param ()

    # Ensure the notification assembly is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        return $script:NotificationManager.GetAllNotificationResults()
    }
    catch {
        Write-Error "Failed to get notification history: $_"
        return $null
    }
}

function Remove-NotificationHistory {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$false)]
        [string]$NotificationId
    )

    # Ensure the notification assembly is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $false
        }
    }

    try {
        if ($NotificationId) {
            return $script:NotificationManager.DeleteNotificationResult($NotificationId)
        }
        else {
            return $script:NotificationManager.DeleteAllNotificationResults()
        }
    }
    catch {
        Write-Error "Failed to remove notification history: $_"
        return $false
    }
}

function Get-InteractiveUserSessions {
    [CmdletBinding()]
    param ()

    # Ensure the notification assembly is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        return $script:NotificationManager.GetInteractiveUserSessions()
    }
    catch {
        Write-Error "Failed to get interactive user sessions: $_"
        return $null
    }
}

function Test-SystemContext {
    [CmdletBinding()]
    param ()

    # Ensure the notification assembly is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $false
        }
    }

    try {
        return $script:NotificationManager.IsRunningAsSystem()
    }
    catch {
        Write-Error "Failed to check system context: $_"
        return $false
    }
}

# Export public functions
Export-ModuleMember -Function Initialize-WindowsNotifications, Show-Notification, Get-NotificationResult, Wait-Notification, Get-NotificationHistory, Remove-NotificationHistory, Get-InteractiveUserSessions, Test-SystemContext
