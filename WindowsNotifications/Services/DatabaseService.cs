using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WindowsNotifications.Models;
using WindowsNotifications.Utils;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Service for managing notification state persistence using LiteDB
    /// </summary>
    internal class DatabaseService
    {
        private readonly string _dbPath;
        private readonly LiteDBEmbedded _liteDb;
        private const string DEFAULT_DB_FOLDER = "%PROGRAMDATA%\\WindowsNotifications";
        private const string DEFAULT_DB_NAME = "notifications.db";

        /// <summary>
        /// Creates a new DatabaseService with the specified database path
        /// </summary>
        /// <param name="dbPath">The path to the database file, or null to use the default</param>
        public DatabaseService(string dbPath = null)
        {
            // Determine the database path
            _dbPath = GetDatabasePath(dbPath);
            
            // Ensure the directory exists
            string directory = Path.GetDirectoryName(_dbPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Initialize LiteDB
            _liteDb = new LiteDBEmbedded(_dbPath);
        }

        /// <summary>
        /// Gets the database path, using the default if none is specified
        /// </summary>
        /// <param name="dbPath">The specified database path, or null to use the default</param>
        /// <returns>The resolved database path</returns>
        private string GetDatabasePath(string dbPath)
        {
            if (!string.IsNullOrEmpty(dbPath))
                return dbPath;

            string folder = Environment.ExpandEnvironmentVariables(DEFAULT_DB_FOLDER);
            return Path.Combine(folder, DEFAULT_DB_NAME);
        }

        /// <summary>
        /// Saves a notification result to the database
        /// </summary>
        /// <param name="result">The notification result to save</param>
        /// <returns>True if the save was successful, false otherwise</returns>
        public bool SaveNotificationResult(NotificationResult result)
        {
            try
            {
                return _liteDb.UpsertNotificationResult(result);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a notification result from the database
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <returns>The notification result, or null if not found</returns>
        public NotificationResult GetNotificationResult(string notificationId)
        {
            try
            {
                return _liteDb.GetNotificationResult(notificationId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all notification results from the database
        /// </summary>
        /// <returns>A list of notification results</returns>
        public List<NotificationResult> GetAllNotificationResults()
        {
            try
            {
                return _liteDb.GetAllNotificationResults();
            }
            catch (Exception)
            {
                return new List<NotificationResult>();
            }
        }

        /// <summary>
        /// Deletes a notification result from the database
        /// </summary>
        /// <param name="notificationId">The ID of the notification</param>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        public bool DeleteNotificationResult(string notificationId)
        {
            try
            {
                return _liteDb.DeleteNotificationResult(notificationId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes all notification results from the database
        /// </summary>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        public bool DeleteAllNotificationResults()
        {
            try
            {
                return _liteDb.DeleteAllNotificationResults();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the path to the database file
        /// </summary>
        /// <returns>The database file path</returns>
        public string GetDatabaseFilePath()
        {
            return _dbPath;
        }
    }
}
