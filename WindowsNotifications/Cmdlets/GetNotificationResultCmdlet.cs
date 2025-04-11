using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets a notification result.</para>
    /// <para type="description">Gets the result of a notification by its ID.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "NotificationResult")]
    [OutputType(typeof(WindowsNotifications.Models.NotificationResult))]
    public class GetNotificationResultCmdlet : PSCmdlet
    {
        private WindowsNotifications.NotificationManager _notificationManager;

        /// <summary>
        /// <para type="description">The ID of the notification.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string NotificationId { get; set; }

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
                var result = _notificationManager.GetNotificationResult(NotificationId);
                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetNotificationResultError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
