#
# WindowsNotifications.psm1
# PowerShell module for Windows notifications
#

# The assembly is loaded automatically by the module manifest (PSD1)

# Module variables
$script:NotificationManager = $null

#
# Public functions
#

function Initialize-WindowsNotifications {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$false)]
        [string]$DatabasePath
    )

    try {
        # Create the notification manager
        if ($DatabasePath) {
            $script:NotificationManager = New-Object WindowsNotifications.NotificationManager($DatabasePath)
        }
        else {
            $script:NotificationManager = New-Object WindowsNotifications.NotificationManager
        }

        Write-Output "Windows Notifications initialized successfully."
        Write-Output "Database path: $($script:NotificationManager.GetDatabaseFilePath())"

        # Check if running as SYSTEM
        $isSystem = $script:NotificationManager.IsRunningAsSystem()
        Write-Output "Running as SYSTEM: $isSystem"

        # Check for interactive user sessions
        $sessions = $script:NotificationManager.GetInteractiveUserSessions()
        Write-Output "Interactive user sessions: $($sessions.Count)"
        foreach ($session in $sessions) {
            Write-Output "  - $session"
        }

        return $true
    }
    catch {
        Write-Error "Failed to initialize NotificationManager: $_"
        return $false
    }
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

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$Async,

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
        [string]$Attribution,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [DateTime]$DeadlineTime,

        [Parameter(Mandatory=$false, ParameterSetName="Simple")]
        [Parameter(Mandatory=$false, ParameterSetName="WithButtons")]
        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [switch]$ShowCountdown,

        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$RebootButtonText = "Reboot Now",

        [Parameter(Mandatory=$false, ParameterSetName="Reboot")]
        [string]$DeferButtonText = "Defer"
    )

    # Ensure the notification manager is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        # Create notification options
        $options = New-Object WindowsNotifications.Models.NotificationOptions
        $options.Title = $Title
        $options.Message = $Message
        $options.Async = $Async.IsPresent
        $options.TimeoutInSeconds = $TimeoutInSeconds

        # Add optional parameters
        if ($LogoImagePath) { $options.LogoImagePath = $LogoImagePath }
        if ($HeroImagePath) { $options.HeroImagePath = $HeroImagePath }
        if ($Attribution) { $options.Attribution = $Attribution }
        if ($DeadlineTime) { $options.DeadlineTime = $DeadlineTime }
        if ($ShowCountdown) { $options.ShowCountdown = $true }

        # Handle different parameter sets
        switch ($PSCmdlet.ParameterSetName) {
            "Simple" {
                # Show a simple notification
                return $script:NotificationManager.ShowNotification($options)
            }
            "WithButtons" {
                # Add buttons
                foreach ($buttonText in $Buttons) {
                    $button = New-Object WindowsNotifications.Models.NotificationButton($buttonText, "button$($options.Buttons.Count)")
                    $options.Buttons.Add($button)
                }

                # Show the notification with buttons
                return $script:NotificationManager.ShowNotification($options)
            }
            "Reboot" {
                # Create deferral options
                $deferralOptions = New-Object WindowsNotifications.Models.DeferralOptions
                $deferralOptions.DeferButtonText = $DeferButtonText
                $options.DeferralOptions = $deferralOptions

                # Add reboot button
                $rebootButton = New-Object WindowsNotifications.Models.NotificationButton($RebootButtonText, "reboot")
                $options.Buttons.Add($rebootButton)

                # Show the reboot notification
                return $script:NotificationManager.ShowNotification($options)
            }
        }
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

    # Ensure the notification manager is initialized
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

    # Ensure the notification manager is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        return $script:NotificationManager.WaitForNotification($NotificationId, $TimeoutInSeconds * 1000)
    }
    catch {
        Write-Error "Failed to wait for notification: $_"
        return $null
    }
}

function Get-AllNotificationResults {
    [CmdletBinding()]
    param ()

    # Ensure the notification manager is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $null
        }
    }

    try {
        return $script:NotificationManager.GetAllNotificationResults()
    }
    catch {
        Write-Error "Failed to get all notification results: $_"
        return $null
    }
}

function Remove-NotificationResult {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string]$NotificationId
    )

    # Ensure the notification manager is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $false
        }
    }

    try {
        return $script:NotificationManager.DeleteNotificationResult($NotificationId)
    }
    catch {
        Write-Error "Failed to remove notification result: $_"
        return $false
    }
}

function Clear-AllNotificationResults {
    [CmdletBinding()]
    param ()

    # Ensure the notification manager is initialized
    if (-not $script:NotificationManager) {
        if (-not (Initialize-WindowsNotifications)) {
            return $false
        }
    }

    try {
        return $script:NotificationManager.DeleteAllNotificationResults()
    }
    catch {
        Write-Error "Failed to clear all notification results: $_"
        return $false
    }
}

function Test-SystemContext {
    [CmdletBinding()]
    param ()

    # Ensure the notification manager is initialized
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

function Get-InteractiveUserSessions {
    [CmdletBinding()]
    param ()

    # Ensure the notification manager is initialized
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

# Export public functions
Export-ModuleMember -Function Initialize-WindowsNotifications, Show-Notification, Get-NotificationResult, Wait-Notification, Get-AllNotificationResults, Remove-NotificationResult, Clear-AllNotificationResults, Test-SystemContext, Get-InteractiveUserSessions
