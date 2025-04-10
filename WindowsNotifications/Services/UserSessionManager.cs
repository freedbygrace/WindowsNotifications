using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Security.Principal;
using System.Diagnostics;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Manages user sessions and detects interactive sessions
    /// </summary>
    internal class UserSessionManager
    {
        #region Win32 API

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            int Reserved,
            int Version,
            ref IntPtr ppSessionInfo,
            ref int pCount);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            int sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out IntPtr ppBuffer,
            out int pBytesReturned);

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_SESSION_INFO
        {
            public int SessionId;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        private enum WTS_INFO_CLASS
        {
            WTSInitialProgram = 0,
            WTSApplicationName = 1,
            WTSWorkingDirectory = 2,
            WTSOEMId = 3,
            WTSSessionId = 4,
            WTSUserName = 5,
            WTSWinStationName = 6,
            WTSDomainName = 7,
            WTSConnectState = 8,
            WTSClientBuildNumber = 9,
            WTSClientName = 10,
            WTSClientDirectory = 11,
            WTSClientProductId = 12,
            WTSClientHardwareId = 13,
            WTSClientAddress = 14,
            WTSClientDisplay = 15,
            WTSClientProtocolType = 16,
            WTSIdleTime = 17,
            WTSLogonTime = 18,
            WTSIncomingBytes = 19,
            WTSOutgoingBytes = 20,
            WTSIncomingFrames = 21,
            WTSOutgoingFrames = 22,
            WTSClientInfo = 23,
            WTSSessionInfo = 24,
            WTSSessionInfoEx = 25,
            WTSConfigInfo = 26,
            WTSValidationInfo = 27,
            WTSSessionAddressV4 = 28,
            WTSIsRemoteSession = 29
        }

        public enum WTS_CONNECTSTATE_CLASS
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

        private const int WTS_CURRENT_SERVER_HANDLE = 0;

        #endregion

        /// <summary>
        /// Represents a user session
        /// </summary>
        public class UserSession
        {
            public int SessionId { get; set; }
            public string UserName { get; set; }
            public string DomainName { get; set; }
            public string StationName { get; set; }
            public WTS_CONNECTSTATE_CLASS State { get; set; }
            public bool IsActiveConsoleSession { get; set; }
            public bool IsRemoteSession { get; set; }

            public override string ToString()
            {
                return $"{DomainName}\\{UserName} (Session: {SessionId}, State: {State}, Console: {IsActiveConsoleSession}, Remote: {IsRemoteSession})";
            }
        }

        /// <summary>
        /// Gets all user sessions
        /// </summary>
        /// <returns>A list of user sessions</returns>
        public List<UserSession> GetUserSessions()
        {
            var sessions = new List<UserSession>();
            IntPtr ppSessionInfo = IntPtr.Zero;
            int count = 0;
            int activeConsoleSessionId = (int)WTSGetActiveConsoleSessionId();

            try
            {
                if (WTSEnumerateSessions(IntPtr.Zero, 0, 1, ref ppSessionInfo, ref count))
                {
                    IntPtr current = ppSessionInfo;

                    for (int i = 0; i < count; i++)
                    {
                        WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure(current, typeof(WTS_SESSION_INFO));
                        current = IntPtr.Add(current, Marshal.SizeOf(typeof(WTS_SESSION_INFO)));

                        // Skip non-active sessions
                        if (si.State != WTS_CONNECTSTATE_CLASS.WTSActive)
                            continue;

                        string userName = GetSessionInfo(si.SessionId, WTS_INFO_CLASS.WTSUserName);
                        string domainName = GetSessionInfo(si.SessionId, WTS_INFO_CLASS.WTSDomainName);
                        bool isRemoteSession = GetIsRemoteSession(si.SessionId);

                        // Skip sessions without a user name (system sessions)
                        if (string.IsNullOrEmpty(userName))
                            continue;

                        sessions.Add(new UserSession
                        {
                            SessionId = si.SessionId,
                            UserName = userName,
                            DomainName = domainName,
                            StationName = si.pWinStationName,
                            State = si.State,
                            IsActiveConsoleSession = si.SessionId == activeConsoleSessionId,
                            IsRemoteSession = isRemoteSession
                        });
                    }
                }
            }
            finally
            {
                if (ppSessionInfo != IntPtr.Zero)
                    WTSFreeMemory(ppSessionInfo);
            }

            return sessions;
        }

        /// <summary>
        /// Gets all interactive user sessions (console or RDP)
        /// </summary>
        /// <returns>A list of interactive user sessions</returns>
        public List<UserSession> GetInteractiveUserSessions()
        {
            return GetUserSessions().Where(s => s.State == WTS_CONNECTSTATE_CLASS.WTSActive).ToList();
        }

        /// <summary>
        /// Gets the active console session, if any
        /// </summary>
        /// <returns>The active console session, or null if none exists</returns>
        public UserSession GetActiveConsoleSession()
        {
            return GetUserSessions().FirstOrDefault(s => s.IsActiveConsoleSession);
        }

        /// <summary>
        /// Gets the session information for the specified session ID and info class
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="infoClass">The information class to retrieve</param>
        /// <returns>The session information</returns>
        private string GetSessionInfo(int sessionId, WTS_INFO_CLASS infoClass)
        {
            IntPtr ppBuffer = IntPtr.Zero;
            int bytesReturned = 0;
            string result = string.Empty;

            try
            {
                if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, infoClass, out ppBuffer, out bytesReturned) && ppBuffer != IntPtr.Zero)
                {
                    result = Marshal.PtrToStringAnsi(ppBuffer);
                }
            }
            finally
            {
                if (ppBuffer != IntPtr.Zero)
                    WTSFreeMemory(ppBuffer);
            }

            return result;
        }

        /// <summary>
        /// Determines if the specified session is a remote session
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <returns>True if the session is remote, false otherwise</returns>
        private bool GetIsRemoteSession(int sessionId)
        {
            IntPtr ppBuffer = IntPtr.Zero;
            int bytesReturned = 0;
            bool result = false;

            try
            {
                if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSIsRemoteSession, out ppBuffer, out bytesReturned) && ppBuffer != IntPtr.Zero)
                {
                    result = Marshal.ReadInt32(ppBuffer) != 0;
                }
            }
            finally
            {
                if (ppBuffer != IntPtr.Zero)
                    WTSFreeMemory(ppBuffer);
            }

            return result;
        }

        /// <summary>
        /// Gets the current interactive user information
        /// </summary>
        /// <returns>A UserSession object representing the current interactive user, or null if none exists</returns>
        public UserSession GetCurrentInteractiveUser()
        {
            try
            {
                // First try to get the active console session
                var consoleSession = GetActiveConsoleSession();
                if (consoleSession != null && !string.IsNullOrEmpty(consoleSession.UserName))
                {
                    return consoleSession;
                }

                // If no active console session, try to find any active session
                var activeSessions = GetInteractiveUserSessions();
                if (activeSessions.Count > 0)
                {
                    return activeSessions[0];
                }

                // If no active sessions, try to get the current user
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    if (identity != null && !identity.IsSystem && !identity.IsAnonymous)
                    {
                        string[] parts = identity.Name.Split('\\');
                        string domain = parts.Length > 1 ? parts[0] : "";
                        string username = parts.Length > 1 ? parts[1] : parts[0];

                        return new UserSession
                        {
                            SessionId = -1, // Unknown session ID
                            UserName = username,
                            DomainName = domain,
                            StationName = "Unknown",
                            State = WTS_CONNECTSTATE_CLASS.WTSActive,
                            IsActiveConsoleSession = false,
                            IsRemoteSession = false
                        };
                    }
                }

                // If all else fails, try to get the user from the process owner
                Process currentProcess = Process.GetCurrentProcess();
                string processOwner = GetProcessOwner(currentProcess.Id);
                if (!string.IsNullOrEmpty(processOwner))
                {
                    string[] parts = processOwner.Split('\\');
                    string domain = parts.Length > 1 ? parts[0] : "";
                    string username = parts.Length > 1 ? parts[1] : parts[0];

                    return new UserSession
                    {
                        SessionId = -1, // Unknown session ID
                        UserName = username,
                        DomainName = domain,
                        StationName = "Unknown",
                        State = WTS_CONNECTSTATE_CLASS.WTSActive,
                        IsActiveConsoleSession = false,
                        IsRemoteSession = false
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting current interactive user: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the owner of a process
        /// </summary>
        /// <param name="processId">The process ID</param>
        /// <returns>The process owner in the format DOMAIN\Username</returns>
        private string GetProcessOwner(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                using (var searcher = new System.Management.ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ProcessId = {processId}"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        string[] args = new string[2];
                        obj.InvokeMethod("GetOwner", args);
                        if (!string.IsNullOrEmpty(args[0]))
                        {
                            return $"{args[1]}\\{args[0]}"; // DOMAIN\Username
                        }
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
