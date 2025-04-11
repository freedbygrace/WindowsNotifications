using System;
using System.Management.Automation;

namespace WindowsNotifications.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Tests if running as SYSTEM.</para>
    /// <para type="description">Tests if the current process is running as SYSTEM.</para>
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "SystemContext")]
    [OutputType(typeof(bool))]
    public class TestSystemContextCmdlet : PSCmdlet
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
                var result = _notificationManager.IsRunningAsSystem();
                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "TestSystemContextError", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
