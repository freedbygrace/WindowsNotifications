using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Initializes the Windows Notifications system.</para>
    /// <para type="description">Initializes the Windows Notifications system with the specified database path.</para>
    /// </summary>
    [Cmdlet(VerbsData.Initialize, "WindowsNotifications")]
    [OutputType(typeof(bool))]
    public class InitializeWindowsNotificationsCmdlet : PSCmdlet
    {
        private WindowsNotifications.NotificationManager _notificationManager;

        /// <summary>
        /// <para type="description">The path to the database file.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DatabasePath { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                _notificationManager = string.IsNullOrEmpty(DatabasePath)
                    ? new NotificationManager()
                    : new NotificationManager(DatabasePath);

                WriteVerbose("Windows Notifications initialized successfully.");
                WriteVerbose(string.Format("Database path: {0}", _notificationManager.GetDatabaseFilePath()));

                // Check if running as SYSTEM
                bool isSystem = _notificationManager.IsRunningAsSystem();
                WriteVerbose(string.Format("Running as SYSTEM: {0}", isSystem));

                // Check for interactive user sessions
                var sessions = _notificationManager.GetInteractiveUserSessions();
                WriteVerbose(string.Format("Interactive user sessions: {0}", sessions.Count));
                foreach (var session in sessions)
                {
                    WriteVerbose(string.Format("  - {0}", session));
                }

                WriteObject(true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InitializeWindowsNotificationsError", ErrorCategory.NotSpecified, null));
                WriteObject(false);
            }
        }
    }
}
