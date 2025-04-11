using NUnit.Framework;
using System;
using WindowsNotifications.Models;

namespace WindowsNotifications.Tests
{
    [TestFixture]
    public class NotificationResultTests
    {
        [Test]
        public void NotificationResult_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var result = new NotificationResult();

            // Assert
            Assert.IsNull(result.NotificationId);
            Assert.IsFalse(result.Displayed);
            Assert.IsFalse(result.Activated);
            Assert.IsFalse(result.Dismissed);
            Assert.IsNull(result.ClickedButtonId);
            Assert.IsNull(result.ClickedButtonText);
            Assert.IsNull(result.ClickedButtonArgument);
            Assert.IsTrue((DateTime.Now - result.CreatedTime).TotalSeconds < 1);
            Assert.IsNull(result.InteractionTime);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNull(result.ErrorCode);
            Assert.IsFalse(result.Deferred);
            Assert.IsNull(result.DeferredUntil);
            Assert.IsNull(result.DeferralReason);
            Assert.IsNull(result.DismissalReason);
            Assert.IsNull(result.SystemAction);
            Assert.IsFalse(result.DeadlineReached);
            Assert.IsNull(result.DeadlineReachedTime);
            Assert.IsNull(result.DeadlineAction);
        }

        [Test]
        public void NotificationResult_SetProperties_ValuesAreCorrect()
        {
            // Arrange
            var result = new NotificationResult();
            var createdTime = DateTime.Now.AddMinutes(-5);
            var interactionTime = DateTime.Now.AddMinutes(-2);
            var deferredUntil = DateTime.Now.AddHours(1);
            var deadlineReachedTime = DateTime.Now.AddMinutes(-1);

            // Act
            result.NotificationId = "test-id";
            result.Displayed = true;
            result.Activated = true;
            result.Dismissed = true;
            result.ClickedButtonId = "button1";
            result.ClickedButtonText = "OK";
            result.ClickedButtonArgument = "ok";
            result.CreatedTime = createdTime;
            result.InteractionTime = interactionTime;
            result.ErrorMessage = "Test error";
            result.ErrorCode = "E001";
            result.Deferred = true;
            result.DeferredUntil = deferredUntil;
            result.DeferralReason = "User requested";
            result.DismissalReason = "User dismissed";
            result.SystemAction = "snooze";
            result.DeadlineReached = true;
            result.DeadlineReachedTime = deadlineReachedTime;
            result.DeadlineAction = "restart";

            // Assert
            Assert.AreEqual("test-id", result.NotificationId);
            Assert.IsTrue(result.Displayed);
            Assert.IsTrue(result.Activated);
            Assert.IsTrue(result.Dismissed);
            Assert.AreEqual("button1", result.ClickedButtonId);
            Assert.AreEqual("OK", result.ClickedButtonText);
            Assert.AreEqual("ok", result.ClickedButtonArgument);
            Assert.AreEqual(createdTime, result.CreatedTime);
            Assert.AreEqual(interactionTime, result.InteractionTime);
            Assert.AreEqual("Test error", result.ErrorMessage);
            Assert.AreEqual("E001", result.ErrorCode);
            Assert.IsTrue(result.Deferred);
            Assert.AreEqual(deferredUntil, result.DeferredUntil);
            Assert.AreEqual("User requested", result.DeferralReason);
            Assert.AreEqual("User dismissed", result.DismissalReason);
            Assert.AreEqual("snooze", result.SystemAction);
            Assert.IsTrue(result.DeadlineReached);
            Assert.AreEqual(deadlineReachedTime, result.DeadlineReachedTime);
            Assert.AreEqual("restart", result.DeadlineAction);
        }
    }
}
