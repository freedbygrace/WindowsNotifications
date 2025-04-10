using NUnit.Framework;
using System;
using System.IO;
using WindowsNotifications.Models;

namespace WindowsNotifications.Tests
{
    [TestFixture]
    public class NotificationManagerTests
    {
        [Test]
        public void Constructor_SetsDefaultDatabasePath()
        {
            // Act
            var manager = new SimpleNotificationManager();

            // Assert
            Assert.IsNotNull(manager.DatabasePath);
            Assert.IsTrue(manager.DatabasePath.Contains("WindowsNotifications"));
            Assert.IsTrue(manager.DatabasePath.EndsWith("notifications.db"));
        }

        [Test]
        public void Constructor_SetsCustomDatabasePath()
        {
            // Arrange
            string customPath = Path.Combine(Path.GetTempPath(), "custom.db");

            // Act
            var manager = new SimpleNotificationManager(customPath);

            // Assert
            Assert.AreEqual(customPath, manager.DatabasePath);
        }

        [Test]
        public void ShowNotification_ValidatesOptions()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.ShowNotification(null));

            var emptyOptions = new NotificationOptions();
            Assert.Throws<ArgumentException>(() => manager.ShowNotification(emptyOptions));
        }

        [Test]
        public void ShowNotification_ReturnsValidResult()
        {
            // Arrange
            var manager = new SimpleNotificationManager();
            var options = new NotificationOptions
            {
                Title = "Test Title",
                Message = "Test Message"
            };

            // Act
            var result = manager.ShowNotification(options);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(options.Id, result.NotificationId);
            Assert.IsTrue(result.Displayed);
            Assert.IsNotNull(result.CreatedTime);
        }

        [Test]
        public void ShowSimpleNotification_ReturnsValidResult()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            var result = manager.ShowSimpleNotification("Test Title", "Test Message");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.NotificationId);
            Assert.IsTrue(result.Displayed);
            Assert.IsNotNull(result.CreatedTime);
        }

        [Test]
        public void ShowNotificationWithButtons_ReturnsValidResult()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            var result = manager.ShowNotificationWithButtons("Test Title", "Test Message", "Button 1", "Button 2");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.NotificationId);
            Assert.IsTrue(result.Displayed);
            Assert.IsNotNull(result.CreatedTime);
            Assert.IsTrue(result.Activated);
            Assert.IsNotNull(result.ClickedButtonId);
            Assert.IsNotNull(result.ClickedButtonText);
            Assert.IsNotNull(result.ClickedButtonArgument);
        }

        [Test]
        public void GetDatabaseFilePath_ReturnsDatabasePath()
        {
            // Arrange
            string customPath = Path.Combine(Path.GetTempPath(), "custom.db");
            var manager = new SimpleNotificationManager(customPath);

            // Act
            string path = manager.GetDatabaseFilePath();

            // Assert
            Assert.AreEqual(customPath, path);
        }

        [Test]
        public void IsRunningAsSystem_ReturnsFalse()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            bool isSystem = manager.IsRunningAsSystem();

            // Assert
            Assert.IsFalse(isSystem);
        }

        [Test]
        public void GetInteractiveUserSessions_ReturnsEmptyList()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            var sessions = manager.GetInteractiveUserSessions();

            // Assert
            Assert.IsNotNull(sessions);
            Assert.AreEqual(0, sessions.Count);
        }

        [Test]
        public void GetNotificationResult_ReturnsNull()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            var result = manager.GetNotificationResult("test-id");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void WaitForNotification_ReturnsNull()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            var result = manager.WaitForNotification("test-id", 100);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetAllNotificationResults_ReturnsEmptyList()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            var results = manager.GetAllNotificationResults();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void DeleteNotificationResult_ReturnsTrue()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            bool result = manager.DeleteNotificationResult("test-id");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteAllNotificationResults_ReturnsTrue()
        {
            // Arrange
            var manager = new SimpleNotificationManager();

            // Act
            bool result = manager.DeleteAllNotificationResults();

            // Assert
            Assert.IsTrue(result);
        }
    }
}
