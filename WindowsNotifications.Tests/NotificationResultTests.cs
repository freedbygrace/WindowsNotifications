using NUnit.Framework;
using System;
using WindowsNotifications.Models;

namespace WindowsNotifications.Tests
{
    [TestFixture]
    public class NotificationResultTests
    {
        [Test]
        public void Constructor_SetsNotificationId()
        {
            // Arrange
            string notificationId = "test-id";

            // Act
            var result = new NotificationResult(notificationId);

            // Assert
            Assert.AreEqual(notificationId, result.NotificationId);
        }

        [Test]
        public void Constructor_SetsDefaultValues()
        {
            // Arrange
            string notificationId = "test-id";

            // Act
            var result = new NotificationResult(notificationId);

            // Assert
            Assert.IsFalse(result.Displayed);
            Assert.IsFalse(result.Activated);
            Assert.IsFalse(result.Dismissed);
            Assert.IsNull(result.ClickedButtonId);
            Assert.IsNull(result.ClickedButtonText);
            Assert.IsNull(result.ClickedButtonArgument);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNull(result.InteractionTime);
            // CreatedTime should be close to now
            Assert.Less((DateTime.Now - result.CreatedTime).TotalSeconds, 5);
        }

        [Test]
        public void Error_CreatesErrorResult()
        {
            // Arrange
            string notificationId = "test-id";
            string errorMessage = "Test error message";

            // Act
            var result = NotificationResult.Error(notificationId, errorMessage);

            // Assert
            Assert.AreEqual(notificationId, result.NotificationId);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
            Assert.IsFalse(result.Displayed);
        }

        [Test]
        public void Properties_CanBeSet()
        {
            // Arrange
            var result = new NotificationResult("test-id");
            DateTime interactionTime = DateTime.Now;

            // Act
            result.Displayed = true;
            result.Activated = true;
            result.Dismissed = true;
            result.ClickedButtonId = "button-id";
            result.ClickedButtonText = "Button Text";
            result.ClickedButtonArgument = "button-arg";
            result.ErrorMessage = "Error message";
            result.InteractionTime = interactionTime;

            // Assert
            Assert.IsTrue(result.Displayed);
            Assert.IsTrue(result.Activated);
            Assert.IsTrue(result.Dismissed);
            Assert.AreEqual("button-id", result.ClickedButtonId);
            Assert.AreEqual("Button Text", result.ClickedButtonText);
            Assert.AreEqual("button-arg", result.ClickedButtonArgument);
            Assert.AreEqual("Error message", result.ErrorMessage);
            Assert.AreEqual(interactionTime, result.InteractionTime);
        }
    }
}
