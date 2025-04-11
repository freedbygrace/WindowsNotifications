using System;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Represents the result of a notification interaction.
    /// </summary>
    public class NotificationResult
    {
        /// <summary>
        /// Gets or sets the unique identifier of the notification.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets whether the notification was successfully displayed.
        /// </summary>
        public bool Displayed { get; set; }

        /// <summary>
        /// Gets or sets whether the notification was activated (clicked).
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// Gets or sets whether the notification was dismissed.
        /// </summary>
        public bool Dismissed { get; set; }

        /// <summary>
        /// Gets or sets the ID of the button that was clicked, if any.
        /// </summary>
        public string ClickedButtonId { get; set; }

        /// <summary>
        /// Gets or sets the text of the button that was clicked, if any.
        /// </summary>
        public string ClickedButtonText { get; set; }

        /// <summary>
        /// Gets or sets the argument of the button that was clicked, if any.
        /// </summary>
        public string ClickedButtonArgument { get; set; }

        /// <summary>
        /// Gets or sets the time when the notification was created.
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the time when the notification was interacted with, if any.
        /// </summary>
        public DateTime? InteractionTime { get; set; }

        /// <summary>
        /// Gets or sets any error message that occurred during the notification process.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error code, if an error occurred.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets whether the notification was deferred.
        /// </summary>
        public bool Deferred { get; set; }

        /// <summary>
        /// Gets or sets the time when the notification was deferred until, if applicable.
        /// </summary>
        public DateTime? DeferredUntil { get; set; }

        /// <summary>
        /// Gets or sets the reason for deferral, if applicable.
        /// </summary>
        public string DeferralReason { get; set; }

        /// <summary>
        /// Gets or sets the reason for dismissal, if applicable.
        /// </summary>
        public string DismissalReason { get; set; }

        /// <summary>
        /// Gets or sets the system action that was taken (e.g., snooze, dismiss).
        /// </summary>
        public string SystemAction { get; set; }

        /// <summary>
        /// Gets or sets whether the deadline was reached.
        /// </summary>
        public bool DeadlineReached { get; set; }

        /// <summary>
        /// Gets or sets the time when the deadline was reached, if applicable.
        /// </summary>
        public DateTime? DeadlineReachedTime { get; set; }

        /// <summary>
        /// Gets or sets the action that was taken when the deadline was reached, if applicable.
        /// </summary>
        public string DeadlineAction { get; set; }
    }
}
