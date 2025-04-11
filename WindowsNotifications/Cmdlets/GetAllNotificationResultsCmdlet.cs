using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets all notification results.</para>
    /// <para type="description">Gets all notification results from the database.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AllNotificationResults")]
    [OutputType(typeof(WindowsNotifications.Models.NotificationResult))]
    public class GetAllNotificationResultsCmdlet : PSCmdlet
    {
        private WindowsNotifications.NotificationManager _notificationManager;

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
                var results = _notificationManager.GetAllNotificationResults();
                WriteObject(results, true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetAllNotificationResultsError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
