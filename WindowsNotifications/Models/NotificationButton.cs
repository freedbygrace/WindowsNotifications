namespace WindowsNotifications.Models
{
    /// <summary>
    /// Represents a button in a notification.
    /// </summary>
    public class NotificationButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationButton"/> class.
        /// </summary>
        public NotificationButton()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationButton"/> class with the specified text and ID.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="id">The unique identifier for the button.</param>
        public NotificationButton(string text, string id)
        {
            Text = text;
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationButton"/> class with the specified text, ID, and argument.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="id">The unique identifier for the button.</param>
        /// <param name="argument">The argument to return when the button is clicked.</param>
        public NotificationButton(string text, string id, string argument)
        {
            Text = text;
            Id = id;
            Argument = argument;
        }

        /// <summary>
        /// Gets or sets the text to display on the button.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the button.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the argument to return when the button is clicked.
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// Gets or sets the background color of the button (hex format: #RRGGBB).
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground (text) color of the button (hex format: #RRGGBB).
        /// </summary>
        public string ForegroundColor { get; set; }
    }
}
