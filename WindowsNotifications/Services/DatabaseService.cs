using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsNotifications.Models;
using WindowsNotifications.Utils;

namespace WindowsNotifications.Services
{
    /// <summary>
    /// Service for managing notification data in LiteDB.
    /// </summary>
    internal class DatabaseService
    {
        private readonly string _databasePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        /// <param name="databasePath">The path to the database file.</param>
        public DatabaseService(string databasePath)
        {
            _databasePath = databasePath;
            EnsureDatabaseDirectoryExists();
        }

        /// <summary>
        /// Saves a notification result to the database.
        /// </summary>
        /// <param name="result">The notification result to save.</param>
        /// <returns>True if the result was saved successfully, false otherwise.</returns>
        public bool SaveNotificationResult(NotificationResult result)
        {
            try
            {
                using (var db = LiteDBEmbedded.GetDatabase(_databasePath))
                {
                    var collection = db.GetCollection<NotificationResult>("notifications");
                    
                    // Create index on NotificationId
                    collection.EnsureIndex(x => x.NotificationId);
                    
                    // Check if the notification already exists
                    var existingResult = collection.FindOne(x => x.NotificationId == result.NotificationId);
                    
                    if (existingResult != null)
                    {
                        // Update existing notification
                        collection.Update(result);
                    }
                    else
                    {
                        // Insert new notification
                        collection.Insert(result);
                    }
                    
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a notification result from the database.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification.</param>
        /// <returns>The notification result, or null if not found.</returns>
        public NotificationResult GetNotificationResult(string notificationId)
        {
            try
            {
                using (var db = LiteDBEmbedded.GetDatabase(_databasePath))
                {
                    var collection = db.GetCollection<NotificationResult>("notifications");
                    return collection.FindOne(x => x.NotificationId == notificationId);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all notification results from the database.
        /// </summary>
        /// <returns>A list of notification results.</returns>
        public List<NotificationResult> GetAllNotificationResults()
        {
            try
            {
                using (var db = LiteDBEmbedded.GetDatabase(_databasePath))
                {
                    var collection = db.GetCollection<NotificationResult>("notifications");
                    return collection.FindAll().ToList();
                }
            }
            catch (Exception)
            {
                return new List<NotificationResult>();
            }
        }

        /// <summary>
        /// Deletes a notification result from the database.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification.</param>
        /// <returns>True if the notification was deleted, false otherwise.</returns>
        public bool DeleteNotificationResult(string notificationId)
        {
            try
            {
                using (var db = LiteDBEmbedded.GetDatabase(_databasePath))
                {
                    var collection = db.GetCollection<NotificationResult>("notifications");
                    return collection.Delete(x => x.NotificationId == notificationId) > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes all notification results from the database.
        /// </summary>
        /// <returns>True if all notifications were deleted, false otherwise.</returns>
        public bool DeleteAllNotificationResults()
        {
            try
            {
                using (var db = LiteDBEmbedded.GetDatabase(_databasePath))
                {
                    var collection = db.GetCollection<NotificationResult>("notifications");
                    collection.DeleteAll();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void EnsureDatabaseDirectoryExists()
        {
            string directory = Path.GetDirectoryName(_databasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
