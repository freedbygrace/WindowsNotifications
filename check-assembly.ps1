Add-Type -Path 'WindowsNotifications\bin\Release\WindowsNotifications.dll'
$assembly = [AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.GetName().Name -eq 'WindowsNotifications' }
$assembly.GetTypes() | Select-Object FullName
