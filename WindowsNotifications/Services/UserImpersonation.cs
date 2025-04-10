using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ComponentModel;
using System.Security.Permissions;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Provides functionality for impersonating a user from SYSTEM context
    /// </summary>
    internal class UserImpersonation : IDisposable
    {
        #region Win32 API

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateToken(
            IntPtr ExistingTokenHandle,
            int ImpersonationLevel,
            out IntPtr DuplicateTokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(uint sessionId, out IntPtr phToken);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_PROVIDER_WINNT50 = 3;
        private const int SECURITY_IMPERSONATION = 2;

        #endregion

        private IntPtr _userToken = IntPtr.Zero;
        private IntPtr _impersonationToken = IntPtr.Zero;
        private IntPtr _environmentBlock = IntPtr.Zero;
        private bool _disposed = false;
        private bool _isImpersonating = false;

        /// <summary>
        /// Gets whether the current process is running as SYSTEM
        /// </summary>
        public static bool IsRunningAsSystem
        {
            get
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    return identity.User.IsWellKnown(WellKnownSidType.LocalSystemSid);
                }
            }
        }

        /// <summary>
        /// Impersonates a user by session ID
        /// </summary>
        /// <param name="sessionId">The session ID to impersonate</param>
        /// <returns>True if impersonation was successful, false otherwise</returns>
        public bool ImpersonateBySessionId(int sessionId)
        {
            if (_isImpersonating)
                return false;

            try
            {
                if (!WTSQueryUserToken((uint)sessionId, out _userToken))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to get user token for session");
                }

                if (!DuplicateToken(_userToken, SECURITY_IMPERSONATION, out _impersonationToken))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to duplicate token");
                }

                if (!CreateEnvironmentBlock(out _environmentBlock, _impersonationToken, false))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to create environment block");
                }

                if (!ImpersonateLoggedOnUser(_impersonationToken))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to impersonate user");
                }

                _isImpersonating = true;
                return true;
            }
            catch (Exception ex)
            {
                CleanupTokens();
                throw new Exception("Impersonation failed", ex);
            }
        }

        /// <summary>
        /// Impersonates a user by username and password
        /// </summary>
        /// <param name="username">The username to impersonate</param>
        /// <param name="domain">The domain of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>True if impersonation was successful, false otherwise</returns>
        public bool ImpersonateUser(string username, string domain, string password)
        {
            if (_isImpersonating)
                return false;

            try
            {
                if (!LogonUser(username, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out _userToken))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to log on as user");
                }

                if (!DuplicateToken(_userToken, SECURITY_IMPERSONATION, out _impersonationToken))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to duplicate token");
                }

                if (!CreateEnvironmentBlock(out _environmentBlock, _impersonationToken, false))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to create environment block");
                }

                if (!ImpersonateLoggedOnUser(_impersonationToken))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to impersonate user");
                }

                _isImpersonating = true;
                return true;
            }
            catch (Exception ex)
            {
                CleanupTokens();
                throw new Exception("Impersonation failed", ex);
            }
        }

        /// <summary>
        /// Stops impersonating the user and reverts to the original security context
        /// </summary>
        public void StopImpersonation()
        {
            if (_isImpersonating)
            {
                RevertToSelf();
                _isImpersonating = false;
            }

            CleanupTokens();
        }

        /// <summary>
        /// Executes an action while impersonating a user
        /// </summary>
        /// <param name="sessionId">The session ID to impersonate</param>
        /// <param name="action">The action to execute</param>
        /// <returns>True if the action was executed successfully, false otherwise</returns>
        public bool ExecuteAsUser(int sessionId, Action action)
        {
            if (!ImpersonateBySessionId(sessionId))
                return false;

            try
            {
                action();
                return true;
            }
            finally
            {
                StopImpersonation();
            }
        }

        /// <summary>
        /// Executes a function while impersonating a user and returns the result
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="sessionId">The session ID to impersonate</param>
        /// <param name="func">The function to execute</param>
        /// <returns>The result of the function</returns>
        public T ExecuteAsUser<T>(int sessionId, Func<T> func)
        {
            if (!ImpersonateBySessionId(sessionId))
                throw new Exception("Failed to impersonate user");

            try
            {
                return func();
            }
            finally
            {
                StopImpersonation();
            }
        }

        /// <summary>
        /// Cleans up any open tokens
        /// </summary>
        private void CleanupTokens()
        {
            if (_environmentBlock != IntPtr.Zero)
            {
                DestroyEnvironmentBlock(_environmentBlock);
                _environmentBlock = IntPtr.Zero;
            }

            if (_impersonationToken != IntPtr.Zero)
            {
                CloseHandle(_impersonationToken);
                _impersonationToken = IntPtr.Zero;
            }

            if (_userToken != IntPtr.Zero)
            {
                CloseHandle(_userToken);
                _userToken = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Disposes of resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources
        /// </summary>
        /// <param name="disposing">Whether this is being called from Dispose() or the finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                }

                // Dispose unmanaged resources
                StopImpersonation();
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~UserImpersonation()
        {
            Dispose(false);
        }
    }
}
