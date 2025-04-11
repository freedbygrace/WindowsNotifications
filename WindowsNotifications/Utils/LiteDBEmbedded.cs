using System;
using System.IO;
using System.Reflection;

namespace WindowsNotifications.Utils
{
    /// <summary>
    /// Utility for working with embedded LiteDB.
    /// </summary>
    internal static class LiteDBEmbedded
    {
        private static readonly object _lockObject = new object();
        private static Assembly _liteDbAssembly;

        /// <summary>
        /// Gets a LiteDB database instance.
        /// </summary>
        /// <param name="databasePath">The path to the database file.</param>
        /// <returns>A LiteDB database instance.</returns>
        public static dynamic GetDatabase(string databasePath)
        {
            EnsureLiteDbLoaded();

            Type liteDbType = _liteDbAssembly.GetType("LiteDB.LiteDatabase");
            return Activator.CreateInstance(liteDbType, databasePath);
        }

        private static void EnsureLiteDbLoaded()
        {
            if (_liteDbAssembly != null)
            {
                return;
            }

            lock (_lockObject)
            {
                if (_liteDbAssembly != null)
                {
                    return;
                }

                // Load the embedded LiteDB assembly
                byte[] liteDbBytes = AssemblyLoader.GetEmbeddedResourceBytes("WindowsNotifications.Resources.LiteDB.dll");
                _liteDbAssembly = Assembly.Load(liteDbBytes);
            }
        }
    }
}
