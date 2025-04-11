using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Service for managing user sessions and impersonation.
    /// </summary>
    internal class UserSessionManager
    {
        /// <summary>
        /// Checks if the current process is running as SYSTEM.
        /// </summary>
        /// <returns>True if running as SYSTEM, false otherwise.</returns>
        public bool IsRunningAsSystem()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                return identity.IsSystem;
            }
        }

        /// <summary>
        /// Checks if there's an interactive user session.
        /// </summary>
        /// <returns>True if there's an interactive user session, false otherwise.</returns>
        public bool HasInteractiveUserSession()
        {
            return GetInteractiveUserSessions().Count > 0;
        }

        /// <summary>
        /// Gets all interactive user sessions.
        /// </summary>
        /// <returns>A list of interactive user sessions.</returns>
        public List<string> GetInteractiveUserSessions()
        {
            List<string> sessions = new List<string>();

            try
            {
                IntPtr serverHandle = IntPtr.Zero;
                IntPtr sessionInfo = IntPtr.Zero;
                int sessionCount = 0;
                int retVal = WTSEnumerateSessions(serverHandle, 0, 1, ref sessionInfo, ref sessionCount);

                if (retVal != 0)
                {
                    int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                    IntPtr currentSession = sessionInfo;

                    for (int i = 0; i < sessionCount; i++)
                    {
                        WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure(currentSession, typeof(WTS_SESSION_INFO));
                        currentSession = IntPtr.Add(currentSession, dataSize);

                        if (si.State == WTS_CONNECTSTATE_CLASS.WTSActive)
                        {
                            string userName = GetUserNameFromSessionId(si.SessionID);
                            if (!string.IsNullOrEmpty(userName) && !userName.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
                            {
                                sessions.Add(userName);
                            }
                        }
                    }

                    WTSFreeMemory(sessionInfo);
                }
            }
            catch
            {
                // Ignore errors
            }

            return sessions;
        }

        /// <summary>
        /// Runs an action as the interactive user.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="action">The action to run.</param>
        /// <returns>The result of the action.</returns>
        public T RunAsInteractiveUser<T>(Func<T> action)
        {
            if (!IsRunningAsSystem())
            {
                return action();
            }

            List<string> sessions = GetInteractiveUserSessions();
            if (sessions.Count == 0)
            {
                return action();
            }

            return UserImpersonation.RunAsUser(sessions[0], action);
        }

        private string GetUserNameFromSessionId(int sessionId)
        {
            IntPtr buffer = IntPtr.Zero;
            int bytesReturned = 0;
            string userName = string.Empty;

            try
            {
                if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out bytesReturned) && bytesReturned > 1)
                {
                    userName = Marshal.PtrToStringAnsi(buffer);
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    WTSFreeMemory(buffer);
                }
            }

            return userName;
        }

        #region Win32 API

        private enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo,
            WTSSessionInfoEx,
            WTSConfigInfo,
            WTSValidationInfo,
            WTSSessionAddressV4,
            WTSIsRemoteSession
        }

        private enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_SESSION_INFO
        {
            public int SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern int WTSEnumerateSessions(
            IntPtr hServer,
            int Reserved,
            int Version,
            ref IntPtr ppSessionInfo,
            ref int pCount);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            int sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out IntPtr ppBuffer,
            out int pBytesReturned);

        #endregion
    }
}
