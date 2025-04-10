using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsNotifications.Models;

namespace WindowsNotifications
{
    /// <summary>
    /// Simple implementation of the Windows Notifications library
    /// </summary>
    public class SimpleNotificationManager
    {
        private readonly string _databasePath;
        private readonly Dictionary<string, Timer> _reminderTimers = new Dictionary<string, Timer>();

        /// <summary>
        /// Gets or sets the path to the database file
        /// </summary>
        public string DatabasePath { get; private set; }

        /// <summary>
        /// Creates a new SimpleNotificationManager with the default database path
        /// </summary>
        public SimpleNotificationManager() : this(null)
        {
        }

        /// <summary>
        /// Creates a new SimpleNotificationManager with the specified database path
        /// </summary>
        /// <param name="databasePath">The path to the database file, or null to use the default</param>
        public SimpleNotificationManager(string databasePath)
        {
            DatabasePath = databasePath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WindowsNotifications", "notifications.db");
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

            // Create a result
            var result = new NotificationResult(options.Id)
            {
                Displayed = true,
                CreatedTime = DateTime.Now
            };

            // Log the notification
            if (options.EnableLogging)
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Notification displayed: ID={options.Id}, Title={options.Title}";
                Debug.WriteLine(logEntry);
                options.LogAction?.Invoke(logEntry);
            }

            // For this simple implementation, we'll just write to the console
            Console.WriteLine($"Notification: {options.Title}");
            Console.WriteLine($"Message: {options.Message}");
            
            if (options.Buttons.Count > 0)
            {
                Console.WriteLine("Buttons:");
                foreach (var button in options.Buttons)
                {
                    Console.WriteLine($"- {button.Text} (ID: {button.Id})");
                }
            }

            // Simulate user interaction
            if (!options.Async)
            {
                // Simulate a delay
                Thread.Sleep(1000);

                // Simulate clicking the first button if there are any
                if (options.Buttons.Count > 0)
                {
                    var button = options.Buttons[0];
                    result.Activated = true;
                    result.ClickedButtonId = button.Id;
                    result.ClickedButtonText = button.Text;
                    result.ClickedButtonArgument = button.Argument;
                    result.InteractionTime = DateTime.Now;
                }
                else
                {
                    // Simulate dismissal
                    result.Dismissed = true;
                    result.InteractionTime = DateTime.Now;
                }
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
        /// Gets the result of a notification
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <returns>The notification result, or null if not found</returns>
        public NotificationResult GetNotificationResult(string notificationId)
        {
            // In this simple implementation, we always return null
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
            // In this simple implementation, we always return null
            return null;
        }

        /// <summary>
        /// Gets all notification results
        /// </summary>
        /// <returns>A list of notification results</returns>
        public List<NotificationResult> GetAllNotificationResults()
        {
            // In this simple implementation, we always return an empty list
            return new List<NotificationResult>();
        }

        /// <summary>
        /// Deletes a notification result
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        public bool DeleteNotificationResult(string notificationId)
        {
            // In this simple implementation, we always return true
            return true;
        }

        /// <summary>
        /// Deletes all notification results
        /// </summary>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        public bool DeleteAllNotificationResults()
        {
            // In this simple implementation, we always return true
            return true;
        }

        /// <summary>
        /// Gets the path to the database file
        /// </summary>
        /// <returns>The database file path</returns>
        public string GetDatabaseFilePath()
        {
            return DatabasePath;
        }

        /// <summary>
        /// Checks if the current process is running as SYSTEM
        /// </summary>
        /// <returns>True if running as SYSTEM, false otherwise</returns>
        public bool IsRunningAsSystem()
        {
            // In this simple implementation, we always return false
            return false;
        }

        /// <summary>
        /// Gets all interactive user sessions
        /// </summary>
        /// <returns>A list of interactive user sessions</returns>
        public List<string> GetInteractiveUserSessions()
        {
            // In this simple implementation, we always return an empty list
            return new List<string>();
        }
    }
}
