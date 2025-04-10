#
# Module manifest for module 'WindowsNotifications'
#

@{
    # Script module or binary module file associated with this manifest.
    RootModule = 'WindowsNotifications.psm1'
    
    # Version number of this module.
    ModuleVersion = '1.0.0'
    
    # Supported PSEditions
    CompatiblePSEditions = @('Desktop')
    
    # ID used to uniquely identify this module
    GUID = '8a1b3c4d-5e6f-47a8-b9c0-d1e2f3a4b5c6'
    
    # Author of this module
    Author = 'WindowsNotifications'
    
    # Company or vendor of this module
    CompanyName = ''
    
    # Copyright statement for this module
    Copyright = '(c) 2024. All rights reserved.'
    
    # Description of the functionality provided by this module
    Description = 'PowerShell module for displaying Windows Toast Notifications from SYSTEM context to user sessions'
    
    # Minimum version of the Windows PowerShell engine required by this module
    PowerShellVersion = '5.0'
    
    # Name of the Windows PowerShell host required by this module
    # PowerShellHostName = ''
    
    # Minimum version of the Windows PowerShell host required by this module
    # PowerShellHostVersion = ''
    
    # Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
    DotNetFrameworkVersion = '4.7.2'
    
    # Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
    CLRVersion = '4.0'
    
    # Processor architecture (None, X86, Amd64) required by this module
    # ProcessorArchitecture = ''
    
    # Modules that must be imported into the global environment prior to importing this module
    # RequiredModules = @()
    
    # Assemblies that must be loaded prior to importing this module
    # RequiredAssemblies = @()
    
    # Script files (.ps1) that are run in the caller's environment prior to importing this module.
    # ScriptsToProcess = @()
    
    # Type files (.ps1xml) to be loaded when importing this module
    # TypesToProcess = @()
    
    # Format files (.ps1xml) to be loaded when importing this module
    # FormatsToProcess = @()
    
    # Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
    # NestedModules = @()
    
    # Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
    FunctionsToExport = @(
        'Initialize-WindowsNotifications',
        'Show-Notification',
        'Get-NotificationResult',
        'Wait-Notification',
        'Get-NotificationHistory',
        'Remove-NotificationHistory',
        'Get-InteractiveUserSessions',
        'Test-SystemContext'
    )
    
    # Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
    CmdletsToExport = @()
    
    # Variables to export from this module
    VariablesToExport = '*'
    
    # Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
    AliasesToExport = @()
    
    # DSC resources to export from this module
    # DscResourcesToExport = @()
    
    # List of all modules packaged with this module
    # ModuleList = @()
    
    # List of all files packaged with this module
    FileList = @(
        'WindowsNotifications.psm1',
        'WindowsNotifications.psd1',
        'WindowsNotifications.dll'
    )
    
    # Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
    PrivateData = @{
        PSData = @{
            # Tags applied to this module. These help with module discovery in online galleries.
            Tags = @('Windows', 'Notifications', 'Toast', 'System')
            
            # A URL to the license for this module.
            LicenseUri = 'https://github.com/freedbygrace/WindowsNotifications/blob/main/LICENSE'
            
            # A URL to the main website for this project.
            ProjectUri = 'https://github.com/freedbygrace/WindowsNotifications'
            
            # A URL to an icon representing this module.
            # IconUri = ''
            
            # ReleaseNotes of this module
            ReleaseNotes = 'Initial release of the WindowsNotifications module'
        }
    }
    
    # HelpInfo URI of this module
    # HelpInfoURI = ''
    
    # Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
    # DefaultCommandPrefix = ''
}
