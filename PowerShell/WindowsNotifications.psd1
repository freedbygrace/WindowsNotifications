@{
    # Script module or binary module file associated with this manifest.
    RootModule = 'WindowsNotifications.psm1'

    # Version number of this module.
    ModuleVersion = '1.0.0'

    # ID used to uniquely identify this module
    GUID = '12345678-1234-1234-1234-123456789012'

    # Author of this module
    Author = 'Windows Notifications'

    # Company or vendor of this module
    CompanyName = ''

    # Copyright statement for this module
    Copyright = '(c) 2023. All rights reserved.'

    # Description of the functionality provided by this module
    Description = 'Windows Notifications for PowerShell'

    # Minimum version of the Windows PowerShell engine required by this module
    PowerShellVersion = '5.0'

    # Functions to export from this module
    FunctionsToExport = @(
        'Initialize-WindowsNotifications',
        'Show-Notification',
        'Get-NotificationResult',
        'Wait-Notification',
        'Get-AllNotificationResults',
        'Remove-NotificationResult',
        'Clear-AllNotificationResults',
        'Test-SystemContext',
        'Get-InteractiveUserSessions'
    )

    # Cmdlets to export from this module
    CmdletsToExport = @()

    # Variables to export from this module
    VariablesToExport = @()

    # Aliases to export from this module
    AliasesToExport = @()

    # Assemblies that must be loaded prior to importing this module
    RequiredAssemblies = @('WindowsNotifications.dll')

    # Private data to pass to the module specified in RootModule/ModuleToProcess
    PrivateData = @{
        PSData = @{
            # Tags applied to this module. These help with module discovery in online galleries.
            Tags = @('Windows', 'Notification', 'Toast')

            # A URL to the license for this module.
            LicenseUri = ''

            # A URL to the main website for this project.
            ProjectUri = ''

            # A URL to an icon representing this module.
            IconUri = ''

            # ReleaseNotes of this module
            ReleaseNotes = 'Initial release of the Windows Notifications module.'
        }
    }
}
