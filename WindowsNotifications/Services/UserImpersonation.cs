using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Utility for impersonating users.
    /// </summary>
    internal static class UserImpersonation
    {
        /// <summary>
        /// Runs an action as the specified user.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="userName">The name of the user to impersonate.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>The result of the action.</returns>
        public static T RunAsUser<T>(string userName, Func<T> action)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return action();
            }

            IntPtr userToken = IntPtr.Zero;
            IntPtr duplicateToken = IntPtr.Zero;
            WindowsImpersonationContext impersonationContext = null;

            try
            {
                // Get the token for the user
                if (!GetSessionUserToken(ref userToken, userName))
                {
                    return action();
                }

                // Duplicate the token
                if (!DuplicateToken(userToken, 2, ref duplicateToken))
                {
                    return action();
                }

                // Create a WindowsIdentity from the token
                using (WindowsIdentity identity = new WindowsIdentity(duplicateToken))
                {
                    // Impersonate the user
                    impersonationContext = identity.Impersonate();

                    // Run the action
                    return action();
                }
            }
            finally
            {
                // Clean up
                if (impersonationContext != null)
                {
                    impersonationContext.Undo();
                }

                if (userToken != IntPtr.Zero)
                {
                    CloseHandle(userToken);
                }

                if (duplicateToken != IntPtr.Zero)
                {
                    CloseHandle(duplicateToken);
                }
            }
        }

        private static bool GetSessionUserToken(ref IntPtr token, string userName)
        {
            IntPtr wtsToken = IntPtr.Zero;
            int sessionId = -1;

            try
            {
                // Find the session ID for the user
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
                            IntPtr buffer = IntPtr.Zero;
                            int bytesReturned = 0;

                            if (WTSQuerySessionInformation(IntPtr.Zero, si.SessionID, WTS_INFO_CLASS.WTSUserName, out buffer, out bytesReturned) && bytesReturned > 1)
                            {
                                string sessionUserName = Marshal.PtrToStringAnsi(buffer);
                                WTSFreeMemory(buffer);

                                if (string.Equals(sessionUserName, userName, StringComparison.OrdinalIgnoreCase))
                                {
                                    sessionId = si.SessionID;
                                    break;
                                }
                            }
                        }
                    }

                    WTSFreeMemory(sessionInfo);
                }

                if (sessionId == -1)
                {
                    return false;
                }

                // Get the user token for the session
                if (!WTSQueryUserToken(sessionId, ref wtsToken))
                {
                    return false;
                }

                // Duplicate the token
                if (!DuplicateTokenEx(wtsToken, TOKEN_ASSIGN_PRIMARY | TOKEN_ALL_ACCESS, IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, ref token))
                {
                    return false;
                }

                return true;
            }
            finally
            {
                if (wtsToken != IntPtr.Zero)
                {
                    CloseHandle(wtsToken);
                }
            }
        }

        #region Win32 API

        private const int TOKEN_ASSIGN_PRIMARY = 0x0001;
        private const int TOKEN_DUPLICATE = 0x0002;
        private const int TOKEN_IMPERSONATE = 0x0004;
        private const int TOKEN_QUERY = 0x0008;
        private const int TOKEN_QUERY_SOURCE = 0x0010;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const int TOKEN_ADJUST_GROUPS = 0x0040;
        private const int TOKEN_ADJUST_DEFAULT = 0x0080;
        private const int TOKEN_ADJUST_SESSIONID = 0x0100;
        private const int TOKEN_ALL_ACCESS = TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID;

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

        private enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        private enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
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

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(
            int sessionId,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            int dwDesiredAccess,
            IntPtr lpTokenAttributes,
            SECURITY_IMPERSONATION_LEVEL impersonationLevel,
            TOKEN_TYPE tokenType,
            ref IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateToken(
            IntPtr ExistingTokenHandle,
            int SECURITY_IMPERSONATION_LEVEL,
            ref IntPtr DuplicateTokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        #endregion
    }
}
