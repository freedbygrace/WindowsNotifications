using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Clears all notification results.</para>
    /// <para type="description">Clears all notification results from the database.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "AllNotificationResults")]
    [OutputType(typeof(bool))]
    public class ClearAllNotificationResultsCmdlet : PSCmdlet
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
                var result = _notificationManager.DeleteAllNotificationResults();
                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ClearAllNotificationResultsError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
