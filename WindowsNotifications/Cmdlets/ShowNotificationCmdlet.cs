using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Shows a notification.</para>
    /// <para type="description">Shows a notification with the specified options.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Show, "Notification")]
    [OutputType(typeof(WindowsNotifications.Models.NotificationResult))]
    public class ShowNotificationCmdlet : PSCmdlet
    {
        private WindowsNotifications.NotificationManager _notificationManager;

        /// <summary>
        /// <para type="description">The title of the notification.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Title { get; set; }

        /// <summary>
        /// <para type="description">The message body of the notification.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string Message { get; set; }

        /// <summary>
        /// <para type="description">The buttons to display on the notification.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] Buttons { get; set; }

        /// <summary>
        /// <para type="description">Whether to run the notification asynchronously.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Async { get; set; }

        /// <summary>
        /// <para type="description">The timeout in seconds (0 = no timeout).</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int TimeoutInSeconds { get; set; }

        /// <summary>
        /// <para type="description">The path to the logo image.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string LogoImagePath { get; set; }

        /// <summary>
        /// <para type="description">The path to the hero image.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string HeroImagePath { get; set; }

        /// <summary>
        /// <para type="description">The attribution text.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Attribution { get; set; }

        /// <summary>
        /// <para type="description">The deadline time for the notification.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public DateTime? DeadlineTime { get; set; }

        /// <summary>
        /// <para type="description">Whether to show a countdown timer on the notification.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter ShowCountdown { get; set; }

        /// <summary>
        /// <para type="description">The path to the database file.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DatabasePath { get; set; }

        protected override void BeginProcessing()
        {
            _notificationManager = string.IsNullOrEmpty(DatabasePath)
                ? new NotificationManager()
                : new NotificationManager(DatabasePath);
        }

        protected override void ProcessRecord()
        {
            try
            {
                var options = new WindowsNotifications.Models.NotificationOptions
                {
                    Title = Title,
                    Message = Message,
                    Async = Async.IsPresent,
                    TimeoutInSeconds = TimeoutInSeconds,
                    LogoImagePath = LogoImagePath,
                    HeroImagePath = HeroImagePath,
                    Attribution = Attribution,
                    DeadlineTime = DeadlineTime,
                    ShowCountdown = ShowCountdown.IsPresent
                };

                if (Buttons != null && Buttons.Length > 0)
                {
                    for (int i = 0; i < Buttons.Length; i++)
                    {
                        options.Buttons.Add(new WindowsNotifications.Models.NotificationButton(Buttons[i], string.Format("button{0}", i), Buttons[i]));
                    }
                }

                var result = _notificationManager.ShowNotification(options);
                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ShowNotificationError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
