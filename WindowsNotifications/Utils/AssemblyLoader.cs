using System;
using System.IO;
using System.Reflection;

namespace WindowsNotifications.Utils
{
    /// <summary>
    /// Provides utilities for loading assemblies in PowerShell
    /// </summary>
    public static class AssemblyLoader
    {
        /// <summary>
        /// Gets the current assembly as a byte array
        /// </summary>
        /// <returns>The assembly as a byte array</returns>
        public static byte[] GetAssemblyBytes()
        {
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                return File.ReadAllBytes(assemblyPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get assembly bytes", ex);
            }
        }

        /// <summary>
        /// Gets the current assembly as a Base64 encoded string
        /// </summary>
        /// <returns>The assembly as a Base64 encoded string</returns>
        public static string GetAssemblyBase64()
        {
            try
            {
                byte[] bytes = GetAssemblyBytes();
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get assembly as Base64", ex);
            }
        }

        /// <summary>
        /// Gets a PowerShell script for loading the assembly
        /// </summary>
        /// <returns>A PowerShell script for loading the assembly</returns>
        public static string GetPowerShellLoaderScript()
        {
            return @"
function Load-WindowsNotifications {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$false)]
        [string]$AssemblyPath
    )

    try {
        if ($AssemblyPath -and (Test-Path $AssemblyPath)) {
            # Load from file
            $bytes = [System.IO.File]::ReadAllBytes($AssemblyPath)
            $assembly = [System.Reflection.Assembly]::Load($bytes)
        }
        else {
            # Load from embedded Base64 string
            $base64 = 'ASSEMBLY_BASE64_PLACEHOLDER'
            $bytes = [System.Convert]::FromBase64String($base64)
            $assembly = [System.Reflection.Assembly]::Load($bytes)
        }

        # Return the assembly
        return $assembly
    }
    catch {
        Write-Error ""Failed to load WindowsNotifications assembly: $_""
        return $null
    }
}

# Example usage:
# $assembly = Load-WindowsNotifications
# $notificationManager = New-Object WindowsNotifications.NotificationManager
# $options = New-Object WindowsNotifications.Models.NotificationOptions
# $options.Title = ""Test Notification""
# $options.Message = ""This is a test notification""
# $result = $notificationManager.ShowNotification($options)
";
        }
    }
}
