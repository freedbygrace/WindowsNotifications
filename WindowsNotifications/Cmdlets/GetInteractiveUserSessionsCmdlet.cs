using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets interactive user sessions.</para>
    /// <para type="description">Gets all interactive user sessions.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "InteractiveUserSessions")]
    [OutputType(typeof(string))]
    public class GetInteractiveUserSessionsCmdlet : PSCmdlet
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
                var sessions = _notificationManager.GetInteractiveUserSessions();
                WriteObject(sessions, true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetInteractiveUserSessionsError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
