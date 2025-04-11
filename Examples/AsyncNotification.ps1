# Example: Asynchronous Notification
# This script demonstrates how to show an asynchronous notification

# Import the module
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "..\PowerShell\WindowsNotifications.psd1"
Import-Module $modulePath -Force

# Initialize the notification system
$dllPath = Join-Path -Path $PSScriptRoot -ChildPath "..\WindowsNotifications\bin\Release\WindowsNotifications.dll"
Initialize-WindowsNotifications -DllPath $dllPath

# Show an asynchronous notification
$result = Show-Notification -Title "Background Task" -Message "A background task is running..." -Buttons "Cancel", "View Details" -Async

# Display the initial result
Write-Host "Notification displayed: $($result.Displayed)"
Write-Host "Notification ID: $($result.NotificationId)"

# Simulate a background task
Write-Host "Performing background task..."
for ($i = 1; $i -le 5; $i++) {
    Write-Host "Working... ($i/5)"
    Start-Sleep -Seconds 2
    
    # Check if the notification has been interacted with
    $currentResult = Get-NotificationResult -NotificationId $result.NotificationId
    
    if ($currentResult -and ($currentResult.Activated -or $currentResult.Dismissed)) {
        Write-Host "User interacted with the notification"
        
        if ($currentResult.ClickedButtonId) {
            Write-Host "Button clicked: $($currentResult.ClickedButtonText) (ID: $($currentResult.ClickedButtonId))"
            
            # Handle button clicks
            if ($currentResult.ClickedButtonText -eq "Cancel") {
                Write-Host "User cancelled the task"
                break
            } elseif ($currentResult.ClickedButtonText -eq "View Details") {
                Write-Host "User wants to view details"
                # In a real script, you would show details here
            }
        }
        
        break
    }
}

Write-Host "Background task completed"

# Get the final result
$finalResult = Get-NotificationResult -NotificationId $result.NotificationId
if ($finalResult) {
    Write-Host "Final notification state:"
    Write-Host "  Activated: $($finalResult.Activated)"
    Write-Host "  Dismissed: $($finalResult.Dismissed)"
    
    if ($finalResult.ClickedButtonId) {
        Write-Host "  Button clicked: $($finalResult.ClickedButtonText) (ID: $($finalResult.ClickedButtonId))"
    }
}
