using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Waits for a notification to complete.</para>
    /// <para type="description">Waits for a notification to complete and returns the result.</para>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Wait, "Notification")]
    [OutputType(typeof(WindowsNotifications.Models.NotificationResult))]
    public class WaitNotificationCmdlet : PSCmdlet
    {
        private WindowsNotifications.NotificationManager _notificationManager;

        /// <summary>
        /// <para type="description">The ID of the notification.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string NotificationId { get; set; }

        /// <summary>
        /// <para type="description">The timeout in seconds (-1 = wait indefinitely).</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int TimeoutInSeconds { get; set; }

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

            // Set default timeout to -1 (wait indefinitely)
            if (TimeoutInSeconds == 0)
            {
                TimeoutInSeconds = -1;
            }
        }

        protected override void ProcessRecord()
        {
            try
            {
                var result = _notificationManager.WaitForNotification(NotificationId, TimeoutInSeconds * 1000);
                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "WaitNotificationError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
