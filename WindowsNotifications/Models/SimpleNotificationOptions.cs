using System;
using System.Collections.Generic;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Options for configuring a notification
    /// </summary>
    public class NotificationOptions
    {
        /// <summary>
        /// The title of the notification
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The main message body of the notification
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional logo image path
        /// </summary>
        public string LogoImagePath { get; set; }

        /// <summary>
        /// Optional hero image path
        /// </summary>
        public string HeroImagePath { get; set; }

        /// <summary>
        /// Optional attribution text
        /// </summary>
        public string Attribution { get; set; }

        /// <summary>
        /// Optional timeout in seconds. If set to 0, the notification will not timeout.
        /// </summary>
        public int TimeoutInSeconds { get; set; } = 0;

        /// <summary>
        /// Optional list of buttons to display on the notification
        /// </summary>
        public List<NotificationButton> Buttons { get; set; } = new List<NotificationButton>();

        /// <summary>
        /// Whether to run the notification asynchronously
        /// </summary>
        public bool Async { get; set; } = false;

        /// <summary>
        /// Optional unique identifier for the notification
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Optional tag for grouping notifications
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Optional group name for grouping notifications
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Whether to persist the notification state in the database
        /// </summary>
        public bool PersistState { get; set; } = false;

        /// <summary>
        /// Whether to enable logging for this notification
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        /// Optional action to handle logging (if null, logs to Debug output)
        /// </summary>
        public Action<string> LogAction { get; set; }
    }

    /// <summary>
    /// Represents a button on a notification
    /// </summary>
    public class NotificationButton
    {
        /// <summary>
        /// The text to display on the button
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Optional identifier for the button
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Optional argument to pass when the button is clicked
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// Creates a new notification button with the specified text
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="id">Optional identifier for the button</param>
        /// <param name="argument">Optional argument to pass when the button is clicked</param>
        public NotificationButton(string text, string id = null, string argument = null)
        {
            Text = text;
            if (!string.IsNullOrEmpty(id))
                Id = id;
            Argument = argument ?? Id;
        }
    }
}
