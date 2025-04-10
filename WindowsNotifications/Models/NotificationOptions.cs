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
        /// Optional logo image path or URL
        /// </summary>
        public string LogoImagePath { get; set; }

        /// <summary>
        /// Optional hero image path or URL
        /// </summary>
        public string HeroImagePath { get; set; }

        /// <summary>
        /// Optional inline image path or URL
        /// </summary>
        public string InlineImagePath { get; set; }

        /// <summary>
        /// Optional app icon path or URL
        /// </summary>
        public string AppIconPath { get; set; }

        /// <summary>
        /// Optional badge logo path or URL
        /// </summary>
        public string BadgeLogoPath { get; set; }

        /// <summary>
        /// Optional background image path or URL for the toast
        /// </summary>
        public string BackgroundImagePath { get; set; }

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
        /// Optional scenario for the notification (alarm, reminder, incomingCall, urgent)
        /// </summary>
        public string Scenario { get; set; }

        /// <summary>
        /// Optional launch argument for the notification
        /// </summary>
        public string LaunchArgument { get; set; }

        /// <summary>
        /// Optional audio source for the notification
        /// </summary>
        public string AudioSource { get; set; }

        /// <summary>
        /// Whether to loop the audio
        /// </summary>
        public bool LoopAudio { get; set; } = false;

        /// <summary>
        /// Optional custom branding text (e.g., company or department name)
        /// </summary>
        public string BrandingText { get; set; }

        /// <summary>
        /// Optional custom branding color (hex format: #RRGGBB)
        /// </summary>
        public string BrandingColor { get; set; }

        /// <summary>
        /// Optional custom accent color for the notification (hex format: #RRGGBB)
        /// </summary>
        public string AccentColor { get; set; }

        /// <summary>
        /// Optional custom font family for the notification text
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// Whether to use dark theme for the notification
        /// </summary>
        public bool UseDarkTheme { get; set; } = false;

        /// <summary>
        /// Whether to show the notification in silent mode (no sound)
        /// </summary>
        public bool SilentMode { get; set; } = false;

        /// <summary>
        /// Whether to show a progress bar on the notification
        /// </summary>
        public bool ShowProgressBar { get; set; } = false;

        /// <summary>
        /// The progress value (0-100) for the progress bar
        /// </summary>
        public int ProgressValue { get; set; } = 0;

        /// <summary>
        /// The status text for the progress bar
        /// </summary>
        public string ProgressStatus { get; set; }

        /// <summary>
        /// Whether to show a countdown timer on the notification
        /// </summary>
        public bool ShowCountdown { get; set; } = false;

        /// <summary>
        /// The deadline time for the notification
        /// </summary>
        public DateTime? DeadlineTime { get; set; }

        /// <summary>
        /// The action to take when the deadline is reached
        /// </summary>
        public DeadlineAction DeadlineAction { get; set; }

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
        /// Optional deferral options for notifications that can be deferred
        /// </summary>
        public DeferralOptions DeferralOptions { get; set; }

        /// <summary>
        /// Whether to show a reminder if the notification is not interacted with
        /// </summary>
        public bool ShowReminder { get; set; } = false;

        /// <summary>
        /// Time in minutes after which to show a reminder if ShowReminder is true
        /// </summary>
        public int ReminderTimeInMinutes { get; set; } = 60;

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

        /// <summary>
        /// Optional action to execute when the notification is activated
        /// </summary>
        public Action<NotificationResult> OnActivated { get; set; }

        /// <summary>
        /// Optional action to execute when the notification times out
        /// </summary>
        public Action<NotificationResult> OnTimeout { get; set; }

        /// <summary>
        /// Optional action to execute when an error occurs
        /// </summary>
        public Action<NotificationResult> OnError { get; set; }

        /// <summary>
        /// Optional deadline time for the notification
        /// </summary>
        public DateTime? DeadlineTime { get; set; }

        /// <summary>
        /// The action to take when the deadline is reached
        /// </summary>
        public DeadlineAction DeadlineAction { get; set; }

        /// <summary>
        /// Whether to show a countdown timer on the notification
        /// </summary>
        public bool ShowCountdown { get; set; } = false;

        /// <summary>
        /// Optional serialized data for reminders
        /// </summary>
        public string ReminderData { get; set; }
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
        /// Optional image URI for the button (file path or URL)
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// Whether this button should appear in the context menu
        /// </summary>
        public bool IsContextMenu { get; set; } = false;

        /// <summary>
        /// Optional tooltip text for the button
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Optional background color for the button (hex format: #RRGGBB)
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Optional text color for the button (hex format: #RRGGBB)
        /// </summary>
        public string TextColor { get; set; }

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

        /// <summary>
        /// Creates a new notification button with the specified text and image
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="imageUri">The image URI for the button (file path or URL)</param>
        /// <param name="id">Optional identifier for the button</param>
        /// <param name="argument">Optional argument to pass when the button is clicked</param>
        public NotificationButton(string text, string imageUri, string id = null, string argument = null)
            : this(text, id, argument)
        {
            ImageUri = imageUri;
        }
    }
}
