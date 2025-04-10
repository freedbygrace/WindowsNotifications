using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using WindowsNotifications.Models;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Service for creating and displaying Windows Toast Notifications
    /// </summary>
    internal class ToastNotificationService
    {
        private const string APP_ID = "WindowsNotifications.PowerShell";
        private readonly Dictionary<string, ManualResetEvent> _notificationEvents = new Dictionary<string, ManualResetEvent>();
        private readonly Dictionary<string, NotificationResult> _notificationResults = new Dictionary<string, NotificationResult>();

        /// <summary>
        /// Shows a toast notification with the specified options
        /// </summary>
        /// <param name="options">The notification options</param>
        /// <returns>The result of the notification</returns>
        public NotificationResult ShowNotification(NotificationOptions options)
        {
            var result = new NotificationResult(options.Id);

            try
            {
                // Register COM server for notifications
                RegisterComServer();

                // Create the toast XML
                string toastXml = CreateToastXml(options);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(toastXml);

                // Create the toast notification
                ToastNotification toast = new ToastNotification(xmlDoc);

                // Set up event handlers
                var resetEvent = new ManualResetEvent(false);
                _notificationEvents[options.Id] = resetEvent;
                _notificationResults[options.Id] = result;

                // Register event handlers
                toast.Activated += (sender, args) => OnToastActivated(sender, args, options);
                toast.Dismissed += (sender, args) => OnToastDismissed(sender, args, options);
                toast.Failed += (sender, args) => OnToastFailed(sender, args, options);

                // Show the notification
                ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
                result.Displayed = true;

                // If not async, wait for the notification to be interacted with
                if (!options.Async)
                {
                    resetEvent.WaitOne();
                    result = _notificationResults[options.Id];
                }

                return result;
            }
            catch (Exception ex)
            {
                return NotificationResult.Error(options.Id, ex.Message);
            }
        }

        /// <summary>
        /// Gets the result of a notification
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <returns>The notification result, or null if not found</returns>
        public NotificationResult GetNotificationResult(string notificationId)
        {
            if (_notificationResults.ContainsKey(notificationId))
                return _notificationResults[notificationId];

            return null;
        }

        /// <summary>
        /// Waits for a notification to complete
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <param name="timeout">The timeout in milliseconds, or -1 to wait indefinitely</param>
        /// <returns>The notification result, or null if timed out or not found</returns>
        public NotificationResult WaitForNotification(string notificationId, int timeout = -1)
        {
            if (!_notificationEvents.ContainsKey(notificationId))
                return null;

            var resetEvent = _notificationEvents[notificationId];
            if (resetEvent.WaitOne(timeout))
                return _notificationResults[notificationId];

            return null;
        }

        /// <summary>
        /// Creates the XML for a toast notification
        /// </summary>
        /// <param name="options">The notification options</param>
        /// <returns>The toast XML</returns>
        private string CreateToastXml(NotificationOptions options)
        {
            // Create the XML document
            XmlDocument doc = new XmlDocument();

            // Create the toast element
            XmlElement toastElement = doc.CreateElement("toast");
            doc.AppendChild(toastElement);

            // Add optional attributes
            if (!string.IsNullOrEmpty(options.Tag))
                toastElement.SetAttribute("tag", options.Tag);

            if (!string.IsNullOrEmpty(options.Group))
                toastElement.SetAttribute("group", options.Group);

            if (options.TimeoutInSeconds > 0)
                toastElement.SetAttribute("duration", options.TimeoutInSeconds <= 30 ? "short" : "long");

            // Add scenario if specified
            if (!string.IsNullOrEmpty(options.Scenario))
                toastElement.SetAttribute("scenario", options.Scenario);

            // Add launch attribute if specified
            if (!string.IsNullOrEmpty(options.LaunchArgument))
                toastElement.SetAttribute("launch", options.LaunchArgument);

            // Create the visual element
            XmlElement visualElement = doc.CreateElement("visual");
            toastElement.AppendChild(visualElement);

            // Add branding color if specified
            if (!string.IsNullOrEmpty(options.BrandingColor))
                visualElement.SetAttribute("baseUri", options.BrandingColor);

            // Add accent color if specified
            if (!string.IsNullOrEmpty(options.AccentColor))
                visualElement.SetAttribute("addImageQuery", options.AccentColor);

            // Create the binding element
            XmlElement bindingElement = doc.CreateElement("binding");
            bindingElement.SetAttribute("template", "ToastGeneric");
            visualElement.AppendChild(bindingElement);

            // Add dark theme if specified
            if (options.UseDarkTheme)
                bindingElement.SetAttribute("hint-darkTheme", "true");

            // Add custom font family if specified
            if (!string.IsNullOrEmpty(options.FontFamily))
                bindingElement.SetAttribute("hint-textStacking", options.FontFamily);

            // Add title and message
            if (!string.IsNullOrEmpty(options.Title))
            {
                XmlElement titleElement = doc.CreateElement("text");
                titleElement.InnerText = options.Title;
                bindingElement.AppendChild(titleElement);
            }

            if (!string.IsNullOrEmpty(options.Message))
            {
                XmlElement messageElement = doc.CreateElement("text");
                messageElement.InnerText = options.Message;
                bindingElement.AppendChild(messageElement);
            }

            // Add countdown timer if enabled
            if (options.ShowCountdown && options.DeadlineTime.HasValue)
            {
                TimeSpan timeLeft = options.DeadlineTime.Value - DateTime.Now;
                if (timeLeft.TotalSeconds > 0)
                {
                    string countdownText = $"Time remaining: {(int)timeLeft.TotalHours}h {timeLeft.Minutes}m {timeLeft.Seconds}s";
                    XmlElement countdownElement = doc.CreateElement("text");
                    countdownElement.InnerText = countdownText;
                    bindingElement.AppendChild(countdownElement);
                }
            }

            // Add images if specified
            // Logo image (small app logo)
            if (!string.IsNullOrEmpty(options.LogoImagePath))
            {
                bool isUrl = options.LogoImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase);
                if (isUrl || File.Exists(options.LogoImagePath))
                {
                    XmlElement logoElement = doc.CreateElement("image");
                    logoElement.SetAttribute("placement", "appLogoOverride");
                    logoElement.SetAttribute("src", options.LogoImagePath);
                    logoElement.SetAttribute("hint-crop", "circle");
                    bindingElement.AppendChild(logoElement);
                }
            }

            // Hero image (large banner image)
            if (!string.IsNullOrEmpty(options.HeroImagePath))
            {
                bool isUrl = options.HeroImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase);
                if (isUrl || File.Exists(options.HeroImagePath))
                {
                    XmlElement heroElement = doc.CreateElement("image");
                    heroElement.SetAttribute("placement", "hero");
                    heroElement.SetAttribute("src", options.HeroImagePath);
                    bindingElement.AppendChild(heroElement);
                }
            }

            // Inline image (image within the notification content)
            if (!string.IsNullOrEmpty(options.InlineImagePath))
            {
                bool isUrl = options.InlineImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase);
                if (isUrl || File.Exists(options.InlineImagePath))
                {
                    XmlElement inlineElement = doc.CreateElement("image");
                    inlineElement.SetAttribute("src", options.InlineImagePath);
                    bindingElement.AppendChild(inlineElement);
                }
            }

            // App icon (icon displayed in the notification)
            if (!string.IsNullOrEmpty(options.AppIconPath))
            {
                bool isUrl = options.AppIconPath.StartsWith("http", StringComparison.OrdinalIgnoreCase);
                if (isUrl || File.Exists(options.AppIconPath))
                {
                    XmlElement appIconElement = doc.CreateElement("image");
                    appIconElement.SetAttribute("placement", "appIcon");
                    appIconElement.SetAttribute("src", options.AppIconPath);
                    bindingElement.AppendChild(appIconElement);
                }
            }

            // Background image (background of the entire toast)
            if (!string.IsNullOrEmpty(options.BackgroundImagePath))
            {
                bool isUrl = options.BackgroundImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase);
                if (isUrl || File.Exists(options.BackgroundImagePath))
                {
                    XmlElement backgroundElement = doc.CreateElement("image");
                    backgroundElement.SetAttribute("placement", "background");
                    backgroundElement.SetAttribute("src", options.BackgroundImagePath);
                    bindingElement.AppendChild(backgroundElement);
                }
            }

            // Add attribution if specified
            if (!string.IsNullOrEmpty(options.Attribution))
            {
                XmlElement attributionElement = doc.CreateElement("text");
                attributionElement.SetAttribute("placement", "attribution");
                attributionElement.InnerText = options.Attribution;
                bindingElement.AppendChild(attributionElement);
            }

            // Add branding text if specified
            if (!string.IsNullOrEmpty(options.BrandingText))
            {
                XmlElement brandingElement = doc.CreateElement("text");
                brandingElement.SetAttribute("placement", "attribution");
                brandingElement.InnerText = options.BrandingText;
                bindingElement.AppendChild(brandingElement);
            }

            // Add progress bar if specified
            if (options.ShowProgressBar && options.ProgressValue >= 0 && options.ProgressValue <= 100)
            {
                XmlElement progressElement = doc.CreateElement("progress");
                progressElement.SetAttribute("value", (options.ProgressValue / 100.0).ToString("0.00"));
                progressElement.SetAttribute("status", options.ProgressStatus ?? "Progress");
                bindingElement.AppendChild(progressElement);
            }

            // Add actions if there are buttons or deferrals
            if (options.Buttons.Count > 0 || (options.DeferralOptions != null && options.DeferralOptions.Enabled))
            {
                XmlElement actionsElement = doc.CreateElement("actions");
                toastElement.AppendChild(actionsElement);

                // Add deferral button if enabled
                if (options.DeferralOptions != null && options.DeferralOptions.Enabled)
                {
                    // Create the input element for deferral selection
                    XmlElement inputElement = doc.CreateElement("input");
                    inputElement.SetAttribute("id", "deferralSelect");
                    inputElement.SetAttribute("type", "selection");
                    inputElement.SetAttribute("title", options.DeferralOptions.DeferralPrompt);
                    actionsElement.AppendChild(inputElement);

                    // Add deferral options
                    foreach (var deferOption in options.DeferralOptions.DeferralChoices)
                    {
                        XmlElement selectionElement = doc.CreateElement("selection");
                        selectionElement.SetAttribute("id", deferOption.Id);
                        selectionElement.SetAttribute("content", deferOption.Text);
                        inputElement.AppendChild(selectionElement);
                    }

                    // Add the defer action button
                    XmlElement deferActionElement = doc.CreateElement("action");
                    deferActionElement.SetAttribute("activationType", "system");
                    deferActionElement.SetAttribute("arguments", "defer");
                    deferActionElement.SetAttribute("content", options.DeferralOptions.DeferButtonText);
                    deferActionElement.SetAttribute("hint-inputId", "deferralSelect");
                    actionsElement.AppendChild(deferActionElement);
                }

                // Add regular buttons
                foreach (var button in options.Buttons)
                {
                    XmlElement actionElement = doc.CreateElement("action");
                    actionElement.SetAttribute("activationType", "foreground");
                    actionElement.SetAttribute("arguments", button.Id);
                    actionElement.SetAttribute("content", button.Text);

                    // Add optional button attributes
                    if (!string.IsNullOrEmpty(button.ImageUri))
                        actionElement.SetAttribute("imageUri", button.ImageUri);

                    if (button.IsContextMenu)
                        actionElement.SetAttribute("placement", "contextMenu");

                    actionsElement.AppendChild(actionElement);
                }
            }

            // Add audio element if specified
            if (!string.IsNullOrEmpty(options.AudioSource) || options.SilentMode)
            {
                XmlElement audioElement = doc.CreateElement("audio");

                if (options.SilentMode)
                {
                    audioElement.SetAttribute("silent", "true");
                }
                else if (!string.IsNullOrEmpty(options.AudioSource))
                {
                    audioElement.SetAttribute("src", options.AudioSource);

                    if (options.LoopAudio)
                        audioElement.SetAttribute("loop", "true");
                }

                toastElement.AppendChild(audioElement);
            }

            // Return the XML as a string
            return doc.GetXml();
        }

        /// <summary>
        /// Escapes XML special characters
        /// </summary>
        /// <param name="text">The text to escape</param>
        /// <returns>The escaped text</returns>
        private string EscapeXml(string text)
        {
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        /// <summary>
        /// Handles toast activation events
        /// </summary>
        private void OnToastActivated(ToastNotification sender, object args, NotificationOptions options)
        {
            if (!_notificationResults.ContainsKey(options.Id))
                return;

            var result = _notificationResults[options.Id];
            result.Activated = true;
            result.InteractionTime = DateTime.Now;

            // Handle button clicks
            if (args is ToastActivatedEventArgs activatedArgs)
            {
                string argument = activatedArgs.Arguments;

                // Check if this is a custom action
                if (argument == "snooze" || argument == "dismiss")
                {
                    // Handle system actions
                    result.SystemAction = argument;
                }
                else
                {
                    // Handle custom button clicks
                    var button = options.Buttons.FirstOrDefault(b => b.Id == argument);

                    if (button != null)
                    {
                        result.ClickedButtonId = button.Id;
                        result.ClickedButtonText = button.Text;
                        result.ClickedButtonArgument = button.Argument;
                    }

                    // Execute custom action if specified
                    if (options.OnActivated != null)
                    {
                        try
                        {
                            options.OnActivated(result);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error executing OnActivated action: {ex.Message}");
                        }
                    }
                }
            }

            // Log the activation
            LogNotificationEvent(options, "Activated", result.ClickedButtonText ?? "Direct activation");

            // Signal that the notification has been interacted with
            if (_notificationEvents.ContainsKey(options.Id))
                _notificationEvents[options.Id].Set();
        }

        /// <summary>
        /// Handles toast dismissal events
        /// </summary>
        private void OnToastDismissed(ToastNotification sender, ToastDismissedEventArgs args, NotificationOptions options)
        {
            if (!_notificationResults.ContainsKey(options.Id))
                return;

            var result = _notificationResults[options.Id];
            result.Dismissed = true;
            result.InteractionTime = DateTime.Now;
            result.DismissalReason = args.Reason.ToString();

            string dismissReason = "Unknown";

            // Handle different dismissal reasons
            switch (args.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    dismissReason = "Application hidden";
                    break;

                case ToastDismissalReason.UserCanceled:
                    dismissReason = "User dismissed";

                    // Handle deferrals
                    if (options.DeferralOptions != null && options.DeferralOptions.Enabled)
                    {
                        // Get the selected deferral option
                        if (args is ToastDismissedEventArgs deferArgs && deferArgs.GetDeferralSelectionId() != null)
                        {
                            string deferralId = deferArgs.GetDeferralSelectionId();
                            var deferOption = options.DeferralOptions.DeferralChoices.FirstOrDefault(d => d.Id == deferralId);

                            if (deferOption != null)
                            {
                                result.Deferred = true;
                                result.DeferredUntil = DateTime.Now.Add(deferOption.DeferralTime);
                                result.DeferralReason = deferOption.Text;
                                dismissReason = $"Deferred until {result.DeferredUntil} ({deferOption.Text})";

                                // Schedule a reminder if needed
                                if (options.DeferralOptions.ScheduleReminder)
                                {
                                    ScheduleDeferralReminder(options, deferOption);
                                }
                            }
                        }
                    }
                    break;

                case ToastDismissalReason.TimedOut:
                    dismissReason = "Timed out";

                    // Execute timeout action if specified
                    if (options.OnTimeout != null)
                    {
                        try
                        {
                            options.OnTimeout(result);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error executing OnTimeout action: {ex.Message}");
                        }
                    }
                    break;
            }

            // Log the dismissal
            LogNotificationEvent(options, "Dismissed", dismissReason);

            // Signal that the notification has been interacted with
            if (_notificationEvents.ContainsKey(options.Id))
                _notificationEvents[options.Id].Set();
        }

        /// <summary>
        /// Handles toast failure events
        /// </summary>
        private void OnToastFailed(ToastNotification sender, ToastFailedEventArgs args, NotificationOptions options)
        {
            if (!_notificationResults.ContainsKey(options.Id))
                return;

            var result = _notificationResults[options.Id];
            result.ErrorMessage = args.ErrorMessage;
            result.ErrorCode = args.ErrorCode.ToString();

            // Log the failure
            LogNotificationEvent(options, "Failed", $"Error: {args.ErrorMessage} (Code: {args.ErrorCode})");

            // Execute error action if specified
            if (options.OnError != null)
            {
                try
                {
                    options.OnError(result);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error executing OnError action: {ex.Message}");
                }
            }

            // Signal that the notification has failed
            if (_notificationEvents.ContainsKey(options.Id))
                _notificationEvents[options.Id].Set();
        }

        /// <summary>
        /// Logs a notification event
        /// </summary>
        /// <param name="options">The notification options</param>
        /// <param name="eventType">The type of event</param>
        /// <param name="details">Additional details about the event</param>
        private void LogNotificationEvent(NotificationOptions options, string eventType, string details)
        {
            if (!options.EnableLogging)
                return;

            try
            {
                // Get the current user session
                var sessionManager = new UserSessionManager();
                var currentUser = sessionManager.GetCurrentInteractiveUser();
                string userInfo = currentUser != null ? $"{currentUser.DomainName}\\{currentUser.UserName} (Session: {currentUser.SessionId})" : "Unknown user";

                // Create the log entry
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{eventType}] ID: {options.Id}, Title: {options.Title}, User: {userInfo}, Details: {details}";

                // Log to the specified action
                if (options.LogAction != null)
                {
                    options.LogAction(logEntry);
                }
                else
                {
                    // Default logging to debug output
                    System.Diagnostics.Debug.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging notification event: {ex.Message}");
            }
        }

        /// <summary>
        /// Schedules a reminder for a deferred notification
        /// </summary>
        /// <param name="options">The notification options</param>
        /// <param name="deferOption">The selected deferral option</param>
        private void ScheduleDeferralReminder(NotificationOptions options, DeferralOption deferOption)
        {
            try
            {
                // Calculate the reminder time
                DateTime reminderTime = DateTime.Now.Add(deferOption.DeferralTime);

                // Create a task scheduler task
                using (TaskService ts = new TaskService())
                {
                    // Create a new task definition
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = $"Reminder for notification: {options.Title}";

                    // Create a trigger that will fire at the reminder time
                    td.Triggers.Add(new TimeTrigger(reminderTime));

                    // Create an action to show the reminder notification
                    string actionPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    string actionArgs = $"-ExecutionPolicy Bypass -Command \"& {{ $bytes = [System.Convert]::FromBase64String('{options.ReminderData}'); $assembly = [System.Reflection.Assembly]::Load($bytes); $notificationManager = New-Object WindowsNotifications.NotificationManager; $options = [System.Runtime.Serialization.Formatters.Binary.BinaryFormatter]::Deserialize([System.IO.MemoryStream]::new($bytes)); $notificationManager.ShowNotification($options); }}\"";

                    td.Actions.Add(new ExecAction(actionPath, actionArgs, null));

                    // Register the task
                    ts.RootFolder.RegisterTaskDefinition($"WindowsNotification_Reminder_{options.Id}", td);
                }

                // Log the scheduled reminder
                LogNotificationEvent(options, "ReminderScheduled", $"Reminder scheduled for {reminderTime}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling deferral reminder: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers the COM server for notifications
        /// </summary>
        private void RegisterComServer()
        {
            try
            {
                // Register the AppID for the current process
                string appUserModelId = APP_ID;
                SetCurrentProcessExplicitAppUserModelID(appUserModelId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register COM server for notifications", ex);
            }
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);
    }
}
