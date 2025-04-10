using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WindowsNotifications.Models;

namespace WindowsNotifications.Utils
{
    /// <summary>
    /// Provides embedded LiteDB functionality
    /// </summary>
    internal class LiteDBEmbedded
    {
        private readonly string _dbPath;
        private readonly Assembly _liteDbAssembly;
        private readonly Type _liteDatabase;
        private readonly Type _bsonMapper;
        private readonly Type _bsonDocument;
        private readonly Type _query;
        private readonly Type _bsonValue;
        private readonly object _db;
        private readonly object _mapper;

        /// <summary>
        /// Creates a new LiteDBEmbedded instance with the specified database path
        /// </summary>
        /// <param name="dbPath">The path to the database file</param>
        public LiteDBEmbedded(string dbPath)
        {
            _dbPath = dbPath;

            // Load the embedded LiteDB assembly
            _liteDbAssembly = LoadLiteDbAssembly();

            // Get the types we need
            _liteDatabase = _liteDbAssembly.GetType("LiteDB.LiteDatabase");
            _bsonMapper = _liteDbAssembly.GetType("LiteDB.BsonMapper");
            _bsonDocument = _liteDbAssembly.GetType("LiteDB.BsonDocument");
            _query = _liteDbAssembly.GetType("LiteDB.Query");
            _bsonValue = _liteDbAssembly.GetType("LiteDB.BsonValue");

            // Create the mapper
            _mapper = _bsonMapper.GetProperty("Global", BindingFlags.Public | BindingFlags.Static).GetValue(null);

            // Create the database
            _db = Activator.CreateInstance(_liteDatabase, new object[] { _dbPath });

            // Register the NotificationResult type
            RegisterNotificationResultType();
        }

        /// <summary>
        /// Loads the embedded LiteDB assembly
        /// </summary>
        /// <returns>The LiteDB assembly</returns>
        private Assembly LoadLiteDbAssembly()
        {
            try
            {
                // Try to load from embedded resource
                string resourceName = "WindowsNotifications.Resources.LiteDB.dll";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        byte[] assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
                }

                // If not found as embedded resource, try to load from file
                return Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LiteDB.dll"));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load LiteDB assembly", ex);
            }
        }

        /// <summary>
        /// Registers the NotificationResult type with the BsonMapper
        /// </summary>
        private void RegisterNotificationResultType()
        {
            try
            {
                // Get the Entity method
                MethodInfo entityMethod = _bsonMapper.GetMethod("Entity", new Type[] { });
                
                // Create a generic Entity<NotificationResult> method
                MethodInfo genericEntityMethod = entityMethod.MakeGenericMethod(typeof(NotificationResult));
                
                // Call the method to register the type
                genericEntityMethod.Invoke(_mapper, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register NotificationResult type", ex);
            }
        }

        /// <summary>
        /// Inserts or updates a notification result in the database
        /// </summary>
        /// <param name="result">The notification result to save</param>
        /// <returns>True if the operation was successful, false otherwise</returns>
        public bool UpsertNotificationResult(NotificationResult result)
        {
            try
            {
                // Get the collection
                object collection = _liteDatabase.GetMethod("GetCollection", new Type[] { typeof(string) })
                    .Invoke(_db, new object[] { "NotificationResults" });

                // Convert the result to a BsonDocument
                object bsonDoc = _bsonMapper.GetMethod("ToDocument", new Type[] { typeof(object) })
                    .Invoke(_mapper, new object[] { result });

                // Upsert the document
                bool success = (bool)collection.GetType().GetMethod("Upsert", new Type[] { _bsonDocument })
                    .Invoke(collection, new object[] { bsonDoc });

                return success;
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
                // Get the collection
                object collection = _liteDatabase.GetMethod("GetCollection", new Type[] { typeof(string) })
                    .Invoke(_db, new object[] { "NotificationResults" });

                // Create a query for the notification ID
                object query = collection.GetType().GetMethod("FindById")
                    .Invoke(collection, new object[] { CreateBsonValue(notificationId) });

                // Convert the result to a NotificationResult
                if (query != null)
                {
                    return (NotificationResult)_bsonMapper.GetMethod("ToObject", new Type[] { typeof(Type), _bsonDocument })
                        .Invoke(_mapper, new object[] { typeof(NotificationResult), query });
                }

                return null;
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
                // Get the collection
                object collection = _liteDatabase.GetMethod("GetCollection", new Type[] { typeof(string) })
                    .Invoke(_db, new object[] { "NotificationResults" });

                // Find all documents
                object query = collection.GetType().GetMethod("FindAll")
                    .Invoke(collection, null);

                // Convert the results to a list of NotificationResults
                var results = new List<NotificationResult>();
                
                // Enumerate the results
                foreach (object doc in (System.Collections.IEnumerable)query)
                {
                    var result = (NotificationResult)_bsonMapper.GetMethod("ToObject", new Type[] { typeof(Type), _bsonDocument })
                        .Invoke(_mapper, new object[] { typeof(NotificationResult), doc });
                    
                    results.Add(result);
                }

                return results;
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
                // Get the collection
                object collection = _liteDatabase.GetMethod("GetCollection", new Type[] { typeof(string) })
                    .Invoke(_db, new object[] { "NotificationResults" });

                // Delete the document
                bool success = (bool)collection.GetType().GetMethod("Delete", new Type[] { _bsonValue })
                    .Invoke(collection, new object[] { CreateBsonValue(notificationId) });

                return success;
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
                // Get the collection
                object collection = _liteDatabase.GetMethod("GetCollection", new Type[] { typeof(string) })
                    .Invoke(_db, new object[] { "NotificationResults" });

                // Delete all documents
                int count = (int)collection.GetType().GetMethod("DeleteAll")
                    .Invoke(collection, null);

                return count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a BsonValue from a string
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>A BsonValue object</returns>
        private object CreateBsonValue(string value)
        {
            return Activator.CreateInstance(_bsonValue, new object[] { value });
        }
    }
}
