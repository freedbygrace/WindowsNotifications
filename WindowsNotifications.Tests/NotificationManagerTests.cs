using NUnit.Framework;
using System;
using System.IO;
using WindowsNotifications;
using WindowsNotifications.Models;

namespace WindowsNotifications.Tests
{
    [TestFixture]
    public class NotificationManagerTests
    {
        private string _testDbPath;
        private NotificationManager _manager;

        [SetUp]
        public void Setup()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), string.Format("test_notifications_{0}.db", Guid.NewGuid()));
            _manager = new NotificationManager(_testDbPath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_testDbPath))
            {
                try
                {
                    File.Delete(_testDbPath);
                }
                catch
                {
                    // Ignore errors
                }
            }
        }

        [Test]
        public void NotificationManager_Constructor_DefaultPath_IsCorrect()
        {
            // Arrange & Act
            var manager = new NotificationManager();

            // Assert
            string dbPath = manager.GetDatabaseFilePath();
            Assert.IsNotNull(dbPath);
            Assert.IsTrue(dbPath.Contains("WindowsNotifications"));
            Assert.IsTrue(dbPath.EndsWith(".db"));
        }

        [Test]
        public void NotificationManager_Constructor_CustomPath_IsCorrect()
        {
            // Arrange & Act
            var customPath = Path.Combine(Path.GetTempPath(), "custom_notifications.db");
            var manager = new NotificationManager(customPath);

            // Assert
            Assert.AreEqual(customPath, manager.GetDatabaseFilePath());
        }

        [Test]
        public void NotificationManager_IsRunningAsSystem_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool isSystem = _manager.IsRunningAsSystem();

            // Assert
            // This will be false in a test environment, but we're just testing the method returns a value
            Assert.IsFalse(isSystem);
        }

        [Test]
        public void NotificationManager_GetInteractiveUserSessions_ReturnsNonNullList()
        {
            // Arrange & Act
            var sessions = _manager.GetInteractiveUserSessions();

            // Assert
            Assert.IsNotNull(sessions);
            // We can't assert the count because it depends on the environment
        }

        [Test]
        public void NotificationManager_ShowSimpleNotification_ReturnsResult()
        {
            // Arrange
            string title = "Test Title";
            string message = "Test Message";

            // Act
            var result = _manager.ShowSimpleNotification(title, message);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.NotificationId);
        }

        [Test]
        public void NotificationManager_ShowNotificationWithButtons_ReturnsResult()
        {
            // Arrange
            string title = "Test Title";
            string message = "Test Message";
            string[] buttons = new[] { "OK", "Cancel" };

            // Act
            var result = _manager.ShowNotificationWithButtons(title, message, buttons);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.NotificationId);
        }

        [Test]
        public void NotificationManager_ShowRebootNotification_ReturnsResult()
        {
            // Arrange
            string title = "Test Title";
            string message = "Test Message";

            // Act
            var result = _manager.ShowRebootNotification(title, message);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.NotificationId);
        }

        [Test]
        public void NotificationManager_GetNotificationResult_ReturnsNullForNonExistentId()
        {
            // Arrange
            string nonExistentId = Guid.NewGuid().ToString();

            // Act
            var result = _manager.GetNotificationResult(nonExistentId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void NotificationManager_GetAllNotificationResults_ReturnsNonNullList()
        {
            // Arrange & Act
            var results = _manager.GetAllNotificationResults();

            // Assert
            Assert.IsNotNull(results);
        }

        [Test]
        public void NotificationManager_DeleteNotificationResult_ReturnsFalseForNonExistentId()
        {
            // Arrange
            string nonExistentId = Guid.NewGuid().ToString();

            // Act
            bool result = _manager.DeleteNotificationResult(nonExistentId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void NotificationManager_DeleteAllNotificationResults_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _manager.DeleteAllNotificationResults();

            // Assert
            Assert.IsTrue(result);
        }
    }
}
