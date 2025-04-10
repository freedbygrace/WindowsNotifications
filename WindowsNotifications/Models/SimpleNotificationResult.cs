using System;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Represents the result of a notification interaction
    /// </summary>
    public class NotificationResult
    {
        /// <summary>
        /// The unique identifier of the notification
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Whether the notification was successfully displayed
        /// </summary>
        public bool Displayed { get; set; }

        /// <summary>
        /// Whether the notification was activated (clicked)
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// Whether the notification was dismissed
        /// </summary>
        public bool Dismissed { get; set; }

        /// <summary>
        /// The ID of the button that was clicked, if any
        /// </summary>
        public string ClickedButtonId { get; set; }

        /// <summary>
        /// The text of the button that was clicked, if any
        /// </summary>
        public string ClickedButtonText { get; set; }

        /// <summary>
        /// The argument of the button that was clicked, if any
        /// </summary>
        public string ClickedButtonArgument { get; set; }

        /// <summary>
        /// The time when the notification was created
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// The time when the notification was interacted with, if any
        /// </summary>
        public DateTime? InteractionTime { get; set; }

        /// <summary>
        /// Any error message that occurred during the notification process
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new notification result with the specified notification ID
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification</param>
        public NotificationResult(string notificationId)
        {
            NotificationId = notificationId;
        }

        /// <summary>
        /// Creates a new error notification result
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A notification result with the error message</returns>
        public static NotificationResult Error(string notificationId, string errorMessage)
        {
            return new NotificationResult(notificationId)
            {
                ErrorMessage = errorMessage,
                Displayed = false
            };
        }
    }
}
