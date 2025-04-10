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
        /// The error code, if an error occurred
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Whether the notification was deferred
        /// </summary>
        public bool Deferred { get; set; }

        /// <summary>
        /// The time when the notification was deferred until, if applicable
        /// </summary>
        public DateTime? DeferredUntil { get; set; }

        /// <summary>
        /// The reason for deferral, if applicable
        /// </summary>
        public string DeferralReason { get; set; }

        /// <summary>
        /// The reason for dismissal, if applicable
        /// </summary>
        public string DismissalReason { get; set; }

        /// <summary>
        /// The system action that was taken (e.g., snooze, dismiss)
        /// </summary>
        public string SystemAction { get; set; }

        /// <summary>
        /// Whether the deadline was reached
        /// </summary>
        public bool DeadlineReached { get; set; }

        /// <summary>
        /// The time when the deadline was reached, if applicable
        /// </summary>
        public DateTime? DeadlineReachedTime { get; set; }

        /// <summary>
        /// The action that was taken when the deadline was reached, if applicable
        /// </summary>
        public string DeadlineAction { get; set; }

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
