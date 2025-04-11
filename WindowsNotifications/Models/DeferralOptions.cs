using System;
using System.Collections.Generic;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Options for configuring notification deferrals.
    /// </summary>
    public class DeferralOptions
    {
        /// <summary>
        /// Gets or sets whether deferrals are enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of deferrals allowed.
        /// </summary>
        public int MaxDeferrals { get; set; } = 3;

        /// <summary>
        /// Gets or sets the default deferral time in minutes.
        /// </summary>
        public int DefaultDeferralTimeInMinutes { get; set; } = 60;

        /// <summary>
        /// Gets or sets the list of predefined deferral options in minutes.
        /// </summary>
        public List<int> DeferralOptions { get; set; } = new List<int> { 15, 30, 60, 240, 480 };

        /// <summary>
        /// Gets or sets the text for the deferral button.
        /// </summary>
        public string DeferButtonText { get; set; } = "Defer";

        /// <summary>
        /// Gets or sets the text for the deferral dropdown.
        /// </summary>
        public string DeferDropdownText { get; set; } = "Defer for:";

        /// <summary>
        /// Gets or sets the format for the deferral option text.
        /// </summary>
        public string DeferralOptionFormat { get; set; } = "{0} minutes";

        /// <summary>
        /// Gets or sets the format for the deferral option text when the time is in hours.
        /// </summary>
        public string DeferralOptionHourFormat { get; set; } = "{0} hours";

        /// <summary>
        /// Gets or sets the absolute deadline after which no more deferrals are allowed.
        /// </summary>
        public DateTime? AbsoluteDeadline { get; set; }

        /// <summary>
        /// Gets or sets the text to display when the absolute deadline is approaching.
        /// </summary>
        public string DeadlineApproachingText { get; set; } = "Final deadline approaching. No further deferrals will be allowed.";

        /// <summary>
        /// Gets or sets the time in minutes before the absolute deadline when the deadline approaching message should be shown.
        /// </summary>
        public int DeadlineApproachingWarningMinutes { get; set; } = 120;
    }
}
