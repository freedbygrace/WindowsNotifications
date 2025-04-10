using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsNotifications.Models;
using WindowsNotifications.Services;

namespace WindowsNotifications
{
    /// <summary>
    /// Main entry point for the Windows Notifications library
    /// </summary>
    public class NotificationManager
    {
        private readonly UserSessionManager _sessionManager;
        private readonly ToastNotificationService _toastService;
        private readonly DatabaseService _databaseService;
        private readonly Dictionary<string, Timer> _reminderTimers = new Dictionary<string, Timer>();
        private readonly Dictionary<string, Timer> _deadlineTimers = new Dictionary<string, Timer>();
        private readonly Timer _cleanupTimer;

        /// <summary>
        /// Gets or sets the path to the LiteDB database file
        /// </summary>
        public string DatabasePath { get; private set; }

        /// <summary>
        /// Creates a new NotificationManager with the default database path
        /// </summary>
        public NotificationManager() : this(null)
        {
        }

        /// <summary>
        /// Creates a new NotificationManager with the specified database path
        /// </summary>
        /// <param name="databasePath">The path to the LiteDB database file, or null to use the default</param>
        public NotificationManager(string databasePath)
        {
            DatabasePath = databasePath;
            _sessionManager = new UserSessionManager();
            _toastService = new ToastNotificationService();
            _databaseService = new DatabaseService(databasePath);

            // Initialize cleanup timer to run every hour
            _cleanupTimer = new Timer(CleanupExpiredNotifications, null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Shows a notification with the specified options
        /// </summary>
        /// <param name="options">The notification options</param>
        /// <returns>The result of the notification</returns>
        public NotificationResult ShowNotification(NotificationOptions options)
        {
            // Validate options
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(options.Title) && string.IsNullOrEmpty(options.Message))
                throw new ArgumentException("Either Title or Message must be specified");

            // Get interactive user sessions
            var sessions = _sessionManager.GetInteractiveUserSessions();
            if (sessions.Count == 0)
                return NotificationResult.Error(options.Id, "No interactive user sessions found");

            // Show the notification in the first interactive session
            var session = sessions.First();
            NotificationResult result = null;

            using (var impersonation = new UserImpersonation())
            {
                try
                {
                    result = impersonation.ExecuteAsUser(session.SessionId, () => _toastService.ShowNotification(options));
                }
                catch (Exception ex)
                {
                    result = NotificationResult.Error(options.Id, $"Failed to show notification: {ex.Message}");
                }
            }

            // Set up a reminder if requested
            if (options.ShowReminder && result.Displayed && !result.Activated && !result.Dismissed)
            {
                SetupReminderTimer(options);
            }

            // Set up a deadline timer if specified
            if (options.DeadlineTime.HasValue && result.Displayed && !result.Activated && !result.Dismissed)
            {
                SetupDeadlineTimer(options, result);
            }

            // Persist the result if requested
            if (options.PersistState && result != null)
            {
                _databaseService.SaveNotificationResult(result);
            }

            return result;
        }

        /// <summary>
        /// Shows a simple notification with the specified title and message
        /// </summary>
        /// <param name="title">The title of the notification</param>
        /// <param name="message">The message body of the notification</param>
        /// <returns>The result of the notification</returns>
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
        /// Shows a notification with buttons
        /// </summary>
        /// <param name="title">The title of the notification</param>
        /// <param name="message">The message body of the notification</param>
        /// <param name="buttons">The buttons to display</param>
        /// <returns>The result of the notification</returns>
        public NotificationResult ShowNotificationWithButtons(string title, string message, params string[] buttons)
        {
            var options = new NotificationOptions
            {
                Title = title,
                Message = message,
                Buttons = buttons.Select(b => new NotificationButton(b)).ToList()
            };

            return ShowNotification(options);
        }

        /// <summary>
        /// Shows a reboot notification with deferral options
        /// </summary>
        /// <param name="title">The title of the notification</param>
        /// <param name="message">The message body of the notification</param>
        /// <param name="rebootButtonText">The text for the reboot button</param>
        /// <param name="deferButtonText">The text for the defer button</param>
        /// <returns>The result of the notification</returns>
        public NotificationResult ShowRebootNotification(string title, string message, string rebootButtonText = "Reboot Now", string deferButtonText = "Defer")
        {
            var options = new NotificationOptions
            {
                Title = title,
                Message = message,
                Buttons = new List<NotificationButton> { new NotificationButton(rebootButtonText, "reboot") },
                DeferralOptions = new DeferralOptions { DeferButtonText = deferButtonText },
                PersistState = true
            };

            return ShowNotification(options);
        }

        /// <summary>
        /// Gets the result of a notification
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <returns>The notification result, or null if not found</returns>
        public NotificationResult GetNotificationResult(string notificationId)
        {
            // Try to get from the toast service first
            var result = _toastService.GetNotificationResult(notificationId);

            // If not found, try to get from the database
            if (result == null)
                result = _databaseService.GetNotificationResult(notificationId);

            return result;
        }

        /// <summary>
        /// Waits for a notification to complete
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <param name="timeout">The timeout in milliseconds, or -1 to wait indefinitely</param>
        /// <returns>The notification result, or null if timed out or not found</returns>
        public NotificationResult WaitForNotification(string notificationId, int timeout = -1)
        {
            return _toastService.WaitForNotification(notificationId, timeout);
        }

        /// <summary>
        /// Gets all notification results from the database
        /// </summary>
        /// <returns>A list of notification results</returns>
        public List<NotificationResult> GetAllNotificationResults()
        {
            return _databaseService.GetAllNotificationResults();
        }

        /// <summary>
        /// Deletes a notification result from the database
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        public bool DeleteNotificationResult(string notificationId)
        {
            return _databaseService.DeleteNotificationResult(notificationId);
        }

        /// <summary>
        /// Deletes all notification results from the database
        /// </summary>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        public bool DeleteAllNotificationResults()
        {
            return _databaseService.DeleteAllNotificationResults();
        }

        /// <summary>
        /// Gets the path to the database file
        /// </summary>
        /// <returns>The database file path</returns>
        public string GetDatabaseFilePath()
        {
            return _databaseService.GetDatabaseFilePath();
        }

        /// <summary>
        /// Sets up a reminder timer for a notification
        /// </summary>
        /// <param name="options">The notification options</param>
        private void SetupReminderTimer(NotificationOptions options)
        {
            // Create a reminder timer
            var timer = new Timer(state =>
            {
                var reminderOptions = new NotificationOptions
                {
                    Title = $"Reminder: {options.Title}",
                    Message = options.Message,
                    Buttons = options.Buttons,
                    Async = options.Async,
                    Tag = options.Tag,
                    Group = options.Group,
                    LogoImagePath = options.LogoImagePath,
                    HeroImagePath = options.HeroImagePath,
                    Attribution = options.Attribution,
                    TimeoutInSeconds = options.TimeoutInSeconds,
                    PersistState = options.PersistState
                };

                ShowNotification(reminderOptions);
            }, null, options.ReminderTimeInMinutes * 60 * 1000, Timeout.Infinite);

            // Store the timer
            _reminderTimers[options.Id] = timer;
        }

        /// <summary>
        /// Sets up a deadline timer for a notification
        /// </summary>
        /// <param name="options">The notification options</param>
        /// <param name="result">The notification result</param>
        private void SetupDeadlineTimer(NotificationOptions options, NotificationResult result)
        {
            if (!options.DeadlineTime.HasValue)
                return;

            // Calculate time until deadline
            TimeSpan timeUntilDeadline = options.DeadlineTime.Value - DateTime.Now;
            if (timeUntilDeadline.TotalMilliseconds <= 0)
                return; // Deadline already passed

            // Create a deadline timer
            var timer = new Timer(state =>
            {
                try
                {
                    // Get the latest result from the database if persisted
                    NotificationResult currentResult = options.PersistState
                        ? _databaseService.GetNotificationResult(options.Id) ?? result
                        : result;

                    // If the notification has already been interacted with, do nothing
                    if (currentResult.Activated || currentResult.Dismissed)
                        return;

                    // Update the result
                    currentResult.DeadlineReached = true;
                    currentResult.DeadlineReachedTime = DateTime.Now;
                    currentResult.DeadlineAction = options.DeadlineAction?.ActionType.ToString() ?? "None";

                    // Execute the deadline action if specified
                    if (options.DeadlineAction != null && options.DeadlineAction.ActionType != DeadlineActionType.None)
                    {
                        options.DeadlineAction.Execute(currentResult);
                    }

                    // Persist the updated result if requested
                    if (options.PersistState)
                    {
                        _databaseService.SaveNotificationResult(currentResult);
                    }

                    // Log the deadline reached event
                    if (options.EnableLogging && options.LogAction != null)
                    {
                        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DeadlineReached] ID: {options.Id}, Title: {options.Title}, Action: {currentResult.DeadlineAction}";
                        options.LogAction(logEntry);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error executing deadline action: {ex.Message}");
                }
            }, null, (int)timeUntilDeadline.TotalMilliseconds, Timeout.Infinite);

            // Store the timer
            _deadlineTimers[options.Id] = timer;
        }

        /// <summary>
        /// Cleans up expired notifications from the database
        /// </summary>
        private void CleanupExpiredNotifications(object state)
        {
            try
            {
                // Get all notification results from the database
                var results = _databaseService.GetAllNotificationResults();
                if (results == null || results.Count == 0)
                    return;

                // Find expired notifications (older than 30 days)
                var expiredResults = results.Where(r => r.CreatedTime < DateTime.Now.AddDays(-30)).ToList();
                if (expiredResults.Count == 0)
                    return;

                // Delete expired notifications
                foreach (var result in expiredResults)
                {
                    _databaseService.DeleteNotificationResult(result.NotificationId);

                    // Clean up any associated timers
                    if (_reminderTimers.ContainsKey(result.NotificationId))
                    {
                        _reminderTimers[result.NotificationId].Dispose();
                        _reminderTimers.Remove(result.NotificationId);
                    }

                    if (_deadlineTimers.ContainsKey(result.NotificationId))
                    {
                        _deadlineTimers[result.NotificationId].Dispose();
                        _deadlineTimers.Remove(result.NotificationId);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Cleaned up {expiredResults.Count} expired notifications");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cleaning up expired notifications: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if the current process is running as SYSTEM
        /// </summary>
        /// <returns>True if running as SYSTEM, false otherwise</returns>
        public bool IsRunningAsSystem()
        {
            return UserImpersonation.IsRunningAsSystem;
        }

        /// <summary>
        /// Gets all interactive user sessions
        /// </summary>
        /// <returns>A list of interactive user sessions</returns>
        public List<string> GetInteractiveUserSessions()
        {
            return _sessionManager.GetInteractiveUserSessions()
                .Select(s => s.ToString())
                .ToList();
        }
    }
}
