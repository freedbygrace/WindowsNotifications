using System;
using System.IO;
using System.Reflection;

namespace WindowsNotifications.Utils
{
    /// <summary>
    /// Utility for loading assemblies.
    /// </summary>
    internal static class AssemblyLoader
    {
        /// <summary>
        /// Gets the bytes of an embedded resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>The bytes of the resource.</returns>
        public static byte[] GetEmbeddedResourceBytes(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Resource {resourceName} not found.");
                }

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Gets the text of an embedded resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>The text of the resource.</returns>
        public static string GetEmbeddedResourceText(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Resource {resourceName} not found.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
