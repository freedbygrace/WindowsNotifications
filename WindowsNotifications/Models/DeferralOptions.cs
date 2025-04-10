using System;
using System.Collections.Generic;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Options for configuring notification deferrals
    /// </summary>
    public class DeferralOptions
    {
        /// <summary>
        /// Whether deferrals are enabled for this notification
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The maximum number of times the notification can be deferred
        /// </summary>
        public int MaxDeferrals { get; set; } = 3;

        /// <summary>
        /// The available deferral options to show to the user
        /// </summary>
        public List<DeferralOption> DeferralChoices { get; set; } = new List<DeferralOption>();

        /// <summary>
        /// The text to display for the deferral button
        /// </summary>
        public string DeferButtonText { get; set; } = "Defer";

        /// <summary>
        /// The text to display for the deferral selection prompt
        /// </summary>
        public string DeferralPrompt { get; set; } = "Defer until:";

        /// <summary>
        /// Whether to schedule a reminder when the notification is deferred
        /// </summary>
        public bool ScheduleReminder { get; set; } = true;

        /// <summary>
        /// Whether to enforce the maximum number of deferrals
        /// </summary>
        public bool EnforceMaxDeferrals { get; set; } = true;

        /// <summary>
        /// The action to take when the maximum number of deferrals is reached
        /// </summary>
        public DeadlineAction MaxDeferralsAction { get; set; }

        /// <summary>
        /// The current number of times this notification has been deferred
        /// </summary>
        public int CurrentDeferralCount { get; set; } = 0;

        /// <summary>
        /// Creates a new DeferralOptions instance with default values
        /// </summary>
        public DeferralOptions()
        {
            // Add default deferral choices
            DeferralChoices.Add(new DeferralOption("30 minutes", TimeSpan.FromMinutes(30)));
            DeferralChoices.Add(new DeferralOption("1 hour", TimeSpan.FromHours(1)));
            DeferralChoices.Add(new DeferralOption("4 hours", TimeSpan.FromHours(4)));
            DeferralChoices.Add(new DeferralOption("Tomorrow", TimeSpan.FromDays(1)));

            // Set default max deferrals action
            MaxDeferralsAction = new DeadlineAction();
        }

        /// <summary>
        /// Creates a new DeferralOptions instance with custom deferral choices
        /// </summary>
        /// <param name="deferralChoices">The deferral choices to offer</param>
        public DeferralOptions(List<DeferralOption> deferralChoices)
        {
            DeferralChoices = deferralChoices;
        }
    }

    /// <summary>
    /// Represents a single deferral option
    /// </summary>
    public class DeferralOption
    {
        /// <summary>
        /// The text to display for this deferral option
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The time span to defer for
        /// </summary>
        public TimeSpan DeferralTime { get; set; }

        /// <summary>
        /// Optional identifier for this deferral option
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Creates a new deferral option
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="deferralTime">The time span to defer for</param>
        /// <param name="id">Optional identifier</param>
        public DeferralOption(string text, TimeSpan deferralTime, string id = null)
        {
            Text = text;
            DeferralTime = deferralTime;
            if (!string.IsNullOrEmpty(id))
                Id = id;
        }
    }
}
