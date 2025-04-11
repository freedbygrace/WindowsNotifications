using System;
using System.Collections.Generic;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Options for configuring a notification.
    /// </summary>
    public class NotificationOptions
    {
        /// <summary>
        /// Gets or sets the title of the notification.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the main message body of the notification.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the optional logo image path.
        /// </summary>
        public string LogoImagePath { get; set; }

        /// <summary>
        /// Gets or sets the optional hero image path.
        /// </summary>
        public string HeroImagePath { get; set; }

        /// <summary>
        /// Gets or sets the optional attribution text.
        /// </summary>
        public string Attribution { get; set; }

        /// <summary>
        /// Gets or sets the optional timeout in seconds (0 = no timeout).
        /// </summary>
        public int TimeoutInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the optional list of buttons to display.
        /// </summary>
        public List<NotificationButton> Buttons { get; set; } = new List<NotificationButton>();

        /// <summary>
        /// Gets or sets whether to run the notification asynchronously.
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Gets or sets the optional unique identifier for the notification.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the optional tag for grouping notifications.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the optional group name for grouping notifications.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the optional deferral options.
        /// </summary>
        public DeferralOptions DeferralOptions { get; set; }

        /// <summary>
        /// Gets or sets whether to show a reminder if the notification is not interacted with.
        /// </summary>
        public bool ShowReminder { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes after which to show a reminder.
        /// </summary>
        public int ReminderTimeInMinutes { get; set; } = 60;

        /// <summary>
        /// Gets or sets whether to persist the notification state in the database.
        /// </summary>
        public bool PersistState { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to enable logging for this notification.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets the optional action to handle logging.
        /// </summary>
        public Action<string> LogAction { get; set; }

        /// <summary>
        /// Gets or sets the optional action to execute when the notification is activated.
        /// </summary>
        public Action<NotificationResult> OnActivated { get; set; }

        /// <summary>
        /// Gets or sets the optional action to execute when the notification times out.
        /// </summary>
        public Action<NotificationResult> OnTimeout { get; set; }

        /// <summary>
        /// Gets or sets the optional action to execute when an error occurs.
        /// </summary>
        public Action<NotificationResult> OnError { get; set; }

        /// <summary>
        /// Gets or sets the optional deadline time for the notification.
        /// </summary>
        public DateTime? DeadlineTime { get; set; }

        /// <summary>
        /// Gets or sets the action to take when the deadline is reached.
        /// </summary>
        public DeadlineAction DeadlineAction { get; set; }

        /// <summary>
        /// Gets or sets whether to show a countdown timer on the notification.
        /// </summary>
        public bool ShowCountdown { get; set; }
    }
}
