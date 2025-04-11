using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsNotifications.Models;
using WindowsNotifications.Services;

namespace WindowsNotifications
{
    /// <summary>
    /// The main entry point for the Windows Notifications library.
    /// </summary>
    public class NotificationManager
    {
        private readonly string _databasePath;
        private readonly DatabaseService _databaseService;
        private readonly ToastNotificationService _toastService;
        private readonly UserSessionManager _sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationManager"/> class with the default database path.
        /// </summary>
        public NotificationManager() : this(GetDefaultDatabasePath())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationManager"/> class with the specified database path.
        /// </summary>
        /// <param name="databasePath">The path to the database file.</param>
        public NotificationManager(string databasePath)
        {
            _databasePath = databasePath;
            _databaseService = new DatabaseService(_databasePath);
            _toastService = new ToastNotificationService();
            _sessionManager = new UserSessionManager();
        }

        /// <summary>
        /// Shows a notification with the specified options.
        /// </summary>
        /// <param name="options">The notification options.</param>
        /// <returns>The notification result.</returns>
        public NotificationResult ShowNotification(NotificationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // Generate a unique ID if not provided
            if (string.IsNullOrEmpty(options.Id))
            {
                options.Id = Guid.NewGuid().ToString();
            }

            // Log the notification
            LogNotification(options, "Showing notification");

            // Check if there's an interactive user session
            if (!_sessionManager.HasInteractiveUserSession())
            {
                var result = new NotificationResult
                {
                    NotificationId = options.Id,
                    Displayed = false,
                    ErrorMessage = "No interactive user session found",
                    CreatedTime = DateTime.Now
                };

                LogNotification(options, "No interactive user session found, notification not displayed");

                if (options.PersistState)
                {
                    _databaseService.SaveNotificationResult(result);
                }

                return result;
            }

            // Show the notification
            NotificationResult notificationResult;
            if (IsRunningAsSystem())
            {
                // Show notification using impersonation
                notificationResult = _sessionManager.RunAsInteractiveUser(() => _toastService.ShowNotification(options));
            }
            else
            {
                // Show notification directly
                notificationResult = _toastService.ShowNotification(options);
            }

            // Save the result if persistence is enabled
            if (options.PersistState)
            {
                _databaseService.SaveNotificationResult(notificationResult);
            }

            // If not async, wait for the notification to complete
            if (!options.Async)
            {
                notificationResult = WaitForNotification(options.Id);
            }

            return notificationResult;
        }

        /// <summary>
        /// Shows a simple notification with the specified title and message.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message body of the notification.</param>
        /// <returns>The notification result.</returns>
        public NotificationResult ShowSimpleNotification(string title, string message)
        {
            var options = new NotificationOptions
            {
                Title = title,
                Message = message
            };

            return ShowNotification(options);
        }

        /// <summary>
        /// Shows a notification with buttons.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message body of the notification.</param>
        /// <param name="buttons">The buttons to display.</param>
        /// <returns>The notification result.</returns>
        public NotificationResult ShowNotificationWithButtons(string title, string message, params string[] buttons)
        {
            var options = new NotificationOptions
            {
                Title = title,
                Message = message
            };

            if (buttons != null && buttons.Length > 0)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    options.Buttons.Add(new NotificationButton(buttons[i], $"button{i}", buttons[i]));
                }
            }

            return ShowNotification(options);
        }

        /// <summary>
        /// Shows a reboot notification with deferral options.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message body of the notification.</param>
        /// <param name="rebootButtonText">The text for the reboot button.</param>
        /// <param name="deferButtonText">The text for the defer button.</param>
        /// <returns>The notification result.</returns>
        public NotificationResult ShowRebootNotification(string title, string message, string rebootButtonText = "Reboot Now", string deferButtonText = "Defer")
        {
            var options = new NotificationOptions
            {
                Title = title,
                Message = message,
                DeferralOptions = new DeferralOptions
                {
                    Enabled = true,
                    DeferButtonText = deferButtonText
                }
            };

            options.Buttons.Add(new NotificationButton(rebootButtonText, "reboot", "reboot"));

            return ShowNotification(options);
        }

        /// <summary>
        /// Gets the result of a notification.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification.</param>
        /// <returns>The notification result, or null if not found.</returns>
        public NotificationResult GetNotificationResult(string notificationId)
        {
            return _databaseService.GetNotificationResult(notificationId);
        }

        /// <summary>
        /// Waits for a notification to complete.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification.</param>
        /// <param name="timeout">The timeout in milliseconds, or -1 to wait indefinitely.</param>
        /// <returns>The notification result.</returns>
        public NotificationResult WaitForNotification(string notificationId, int timeout = -1)
        {
            DateTime startTime = DateTime.Now;
            NotificationResult result = null;

            while (true)
            {
                result = GetNotificationResult(notificationId);

                if (result != null && (result.Activated || result.Dismissed || result.DeadlineReached || !string.IsNullOrEmpty(result.ErrorMessage)))
                {
                    break;
                }

                if (timeout > 0 && (DateTime.Now - startTime).TotalMilliseconds > timeout)
                {
                    break;
                }

                Thread.Sleep(500);
            }

            return result;
        }

        /// <summary>
        /// Gets all notification results from the database.
        /// </summary>
        /// <returns>A list of notification results.</returns>
        public List<NotificationResult> GetAllNotificationResults()
        {
            return _databaseService.GetAllNotificationResults();
        }

        /// <summary>
        /// Deletes a notification result from the database.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification.</param>
        /// <returns>True if the notification was deleted, false otherwise.</returns>
        public bool DeleteNotificationResult(string notificationId)
        {
            return _databaseService.DeleteNotificationResult(notificationId);
        }

        /// <summary>
        /// Deletes all notification results from the database.
        /// </summary>
        /// <returns>True if all notifications were deleted, false otherwise.</returns>
        public bool DeleteAllNotificationResults()
        {
            return _databaseService.DeleteAllNotificationResults();
        }

        /// <summary>
        /// Gets the path to the database file.
        /// </summary>
        /// <returns>The path to the database file.</returns>
        public string GetDatabaseFilePath()
        {
            return _databasePath;
        }

        /// <summary>
        /// Checks if the current process is running as SYSTEM.
        /// </summary>
        /// <returns>True if running as SYSTEM, false otherwise.</returns>
        public bool IsRunningAsSystem()
        {
            return _sessionManager.IsRunningAsSystem();
        }

        /// <summary>
        /// Gets all interactive user sessions.
        /// </summary>
        /// <returns>A list of interactive user sessions.</returns>
        public List<string> GetInteractiveUserSessions()
        {
            return _sessionManager.GetInteractiveUserSessions();
        }

        private static string GetDefaultDatabasePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string dbDirectory = Path.Combine(appDataPath, "WindowsNotifications");
            
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
            
            return Path.Combine(dbDirectory, "notifications.db");
        }

        private void LogNotification(NotificationOptions options, string message)
        {
            if (options.EnableLogging)
            {
                string logMessage = $"[{DateTime.Now}] [{options.Id}] {message}";
                options.LogAction?.Invoke(logMessage);
            }
        }
    }
}
