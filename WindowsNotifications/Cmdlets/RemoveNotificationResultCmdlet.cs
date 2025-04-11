using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a notification result.</para>
    /// <para type="description">Removes a notification result from the database.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "NotificationResult")]
    [OutputType(typeof(bool))]
    public class RemoveNotificationResultCmdlet : PSCmdlet
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
                var result = _notificationManager.DeleteNotificationResult(NotificationId);
                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveNotificationResultError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
