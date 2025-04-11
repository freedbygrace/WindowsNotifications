using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using WindowsNotifications.Models;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Service for displaying toast notifications.
    /// </summary>
    internal class ToastNotificationService
    {
        /// <summary>
        /// Shows a notification with the specified options.
        /// </summary>
        /// <param name="options">The notification options.</param>
        /// <returns>The notification result.</returns>
        public NotificationResult ShowNotification(NotificationOptions options)
        {
            var result = new NotificationResult
            {
                NotificationId = options.Id,
                CreatedTime = DateTime.Now
            };

            try
            {
                // Create the toast XML
                string toastXml = GenerateToastXml(options);

                // Create a temporary file for the toast XML
                string tempFile = Path.Combine(Path.GetTempPath(), $"toast_{options.Id}.xml");
                File.WriteAllText(tempFile, toastXml);

                // Create a temporary file for the result
                string resultFile = Path.Combine(Path.GetTempPath(), $"result_{options.Id}.json");

                // Create the process arguments
                string arguments = $"-ToastFile \"{tempFile}\" -ResultFile \"{resultFile}\"";

                if (options.TimeoutInSeconds > 0)
                {
                    arguments += $" -Timeout {options.TimeoutInSeconds}";
                }

                if (options.DeadlineTime.HasValue)
                {
                    arguments += $" -Deadline \"{options.DeadlineTime.Value:yyyy-MM-dd HH:mm:ss}\"";
                }

                if (options.ShowCountdown)
                {
                    arguments += " -ShowCountdown";
                }

                // Launch the toast process
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"& {{ Import-Module Microsoft.PowerShell.Management; [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] | Out-Null; [Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime] | Out-Null; $xml = [Windows.Data.Xml.Dom.XmlDocument]::new(); $xml.LoadXml('{toastXml}'); $toast = [Windows.UI.Notifications.ToastNotification]::new($xml); $notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier('WindowsNotifications'); $notifier.Show($toast); }}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                result.Displayed = true;

                // If not async, wait for the process to complete
                if (!options.Async)
                {
                    process.WaitForExit();

                    // Wait for the result file to be created
                    int maxWaitTime = options.TimeoutInSeconds > 0 ? options.TimeoutInSeconds * 1000 : 30000;
                    int waitTime = 0;
                    int sleepTime = 100;

                    while (!File.Exists(resultFile) && waitTime < maxWaitTime)
                    {
                        Thread.Sleep(sleepTime);
                        waitTime += sleepTime;
                    }

                    // Read the result file if it exists
                    if (File.Exists(resultFile))
                    {
                        string resultJson = File.ReadAllText(resultFile);
                        ParseResultJson(resultJson, result);

                        // Delete the result file
                        try
                        {
                            File.Delete(resultFile);
                        }
                        catch { }
                    }
                }

                // Clean up the temporary file
                try
                {
                    File.Delete(tempFile);
                }
                catch { }
            }
            catch (Exception ex)
            {
                result.Displayed = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private string GenerateToastXml(NotificationOptions options)
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<toast>");
            xml.AppendLine("  <visual>");
            xml.AppendLine("    <binding template=\"ToastGeneric\">");
            
            // Title
            xml.AppendLine($"      <text>{EscapeXml(options.Title)}</text>");
            
            // Message
            xml.AppendLine($"      <text>{EscapeXml(options.Message)}</text>");
            
            // Logo image
            if (!string.IsNullOrEmpty(options.LogoImagePath))
            {
                xml.AppendLine($"      <image placement=\"appLogoOverride\" src=\"{EscapeXml(options.LogoImagePath)}\" />");
            }
            
            // Hero image
            if (!string.IsNullOrEmpty(options.HeroImagePath))
            {
                xml.AppendLine($"      <image placement=\"hero\" src=\"{EscapeXml(options.HeroImagePath)}\" />");
            }
            
            // Attribution
            if (!string.IsNullOrEmpty(options.Attribution))
            {
                xml.AppendLine($"      <text placement=\"attribution\">{EscapeXml(options.Attribution)}</text>");
            }
            
            xml.AppendLine("    </binding>");
            xml.AppendLine("  </visual>");
            
            // Actions
            if (options.Buttons.Count > 0 || (options.DeferralOptions != null && options.DeferralOptions.Enabled))
            {
                xml.AppendLine("  <actions>");
                
                // Regular buttons
                foreach (var button in options.Buttons)
                {
                    xml.AppendLine($"    <action content=\"{EscapeXml(button.Text)}\" arguments=\"{EscapeXml(button.Argument)}\" id=\"{EscapeXml(button.Id)}\" />");
                }
                
                // Deferral options
                if (options.DeferralOptions != null && options.DeferralOptions.Enabled)
                {
                    xml.AppendLine($"    <action content=\"{EscapeXml(options.DeferralOptions.DeferButtonText)}\" arguments=\"defer\" id=\"defer\" />");
                    
                    if (options.DeferralOptions.DeferralOptions.Count > 0)
                    {
                        xml.AppendLine($"    <input id=\"deferTime\" type=\"selection\" title=\"{EscapeXml(options.DeferralOptions.DeferDropdownText)}\">");
                        
                        foreach (var deferOption in options.DeferralOptions.DeferralOptions)
                        {
                            string text = deferOption >= 60 
                                ? string.Format(options.DeferralOptions.DeferralOptionHourFormat, deferOption / 60) 
                                : string.Format(options.DeferralOptions.DeferralOptionFormat, deferOption);
                            
                            xml.AppendLine($"      <selection id=\"{deferOption}\" content=\"{EscapeXml(text)}\" />");
                        }
                        
                        xml.AppendLine("    </input>");
                    }
                }
                
                xml.AppendLine("  </actions>");
            }
            
            xml.AppendLine("</toast>");
            
            return xml.ToString();
        }

        private void ParseResultJson(string json, NotificationResult result)
        {
            try
            {
                // Simple JSON parsing without dependencies
                if (json.Contains("\"Activated\":true"))
                {
                    result.Activated = true;
                }

                if (json.Contains("\"Dismissed\":true"))
                {
                    result.Dismissed = true;
                }

                // Extract button ID
                int buttonIdStart = json.IndexOf("\"ClickedButtonId\":\"");
                if (buttonIdStart >= 0)
                {
                    buttonIdStart += "\"ClickedButtonId\":\"".Length;
                    int buttonIdEnd = json.IndexOf("\"", buttonIdStart);
                    if (buttonIdEnd > buttonIdStart)
                    {
                        result.ClickedButtonId = json.Substring(buttonIdStart, buttonIdEnd - buttonIdStart);
                    }
                }

                // Extract button text
                int buttonTextStart = json.IndexOf("\"ClickedButtonText\":\"");
                if (buttonTextStart >= 0)
                {
                    buttonTextStart += "\"ClickedButtonText\":\"".Length;
                    int buttonTextEnd = json.IndexOf("\"", buttonTextStart);
                    if (buttonTextEnd > buttonTextStart)
                    {
                        result.ClickedButtonText = json.Substring(buttonTextStart, buttonTextEnd - buttonTextStart);
                    }
                }

                // Extract button argument
                int buttonArgStart = json.IndexOf("\"ClickedButtonArgument\":\"");
                if (buttonArgStart >= 0)
                {
                    buttonArgStart += "\"ClickedButtonArgument\":\"".Length;
                    int buttonArgEnd = json.IndexOf("\"", buttonArgStart);
                    if (buttonArgEnd > buttonArgStart)
                    {
                        result.ClickedButtonArgument = json.Substring(buttonArgStart, buttonArgEnd - buttonArgStart);
                    }
                }

                // Extract interaction time
                int interactionTimeStart = json.IndexOf("\"InteractionTime\":\"");
                if (interactionTimeStart >= 0)
                {
                    interactionTimeStart += "\"InteractionTime\":\"".Length;
                    int interactionTimeEnd = json.IndexOf("\"", interactionTimeStart);
                    if (interactionTimeEnd > interactionTimeStart)
                    {
                        string timeStr = json.Substring(interactionTimeStart, interactionTimeEnd - interactionTimeStart);
                        if (DateTime.TryParse(timeStr, out DateTime interactionTime))
                        {
                            result.InteractionTime = interactionTime;
                        }
                    }
                }

                // Check for deferral
                if (result.ClickedButtonId == "defer")
                {
                    result.Deferred = true;

                    // Extract deferral time
                    int deferTimeStart = json.IndexOf("\"DeferredUntil\":\"");
                    if (deferTimeStart >= 0)
                    {
                        deferTimeStart += "\"DeferredUntil\":\"".Length;
                        int deferTimeEnd = json.IndexOf("\"", deferTimeStart);
                        if (deferTimeEnd > deferTimeStart)
                        {
                            string timeStr = json.Substring(deferTimeStart, deferTimeEnd - deferTimeStart);
                            if (DateTime.TryParse(timeStr, out DateTime deferredUntil))
                            {
                                result.DeferredUntil = deferredUntil;
                            }
                        }
                    }
                }

                // Check for deadline reached
                if (json.Contains("\"DeadlineReached\":true"))
                {
                    result.DeadlineReached = true;

                    // Extract deadline reached time
                    int deadlineTimeStart = json.IndexOf("\"DeadlineReachedTime\":\"");
                    if (deadlineTimeStart >= 0)
                    {
                        deadlineTimeStart += "\"DeadlineReachedTime\":\"".Length;
                        int deadlineTimeEnd = json.IndexOf("\"", deadlineTimeStart);
                        if (deadlineTimeEnd > deadlineTimeStart)
                        {
                            string timeStr = json.Substring(deadlineTimeStart, deadlineTimeEnd - deadlineTimeStart);
                            if (DateTime.TryParse(timeStr, out DateTime deadlineReachedTime))
                            {
                                result.DeadlineReachedTime = deadlineReachedTime;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }
        }

        private string EscapeXml(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
