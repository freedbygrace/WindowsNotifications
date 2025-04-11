using NUnit.Framework;
using System;
using WindowsNotifications.Models;

namespace WindowsNotifications.Tests
{
    [TestFixture]
    public class NotificationOptionsTests
    {
        [Test]
        public void NotificationOptions_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var options = new NotificationOptions();

            // Assert
            Assert.IsNotNull(options.Buttons);
            Assert.AreEqual(0, options.Buttons.Count);
            Assert.IsFalse(options.Async);
            Assert.IsNotNull(options.Id);
            Assert.IsTrue(Guid.TryParse(options.Id, out _));
            Assert.IsTrue(options.PersistState);
            Assert.AreEqual(60, options.ReminderTimeInMinutes);
            Assert.IsFalse(options.ShowReminder);
            Assert.IsFalse(options.ShowCountdown);
            Assert.IsFalse(options.EnableLogging);
            Assert.IsNull(options.LogAction);
            Assert.IsNull(options.OnActivated);
            Assert.IsNull(options.OnTimeout);
            Assert.IsNull(options.OnError);
            Assert.IsNull(options.DeadlineTime);
            Assert.IsNull(options.DeadlineAction);
        }

        [Test]
        public void NotificationOptions_SetProperties_ValuesAreCorrect()
        {
            // Arrange
            var options = new NotificationOptions();
            var button = new NotificationButton("Test", "test");
            var deferralOptions = new DeferralOptions();
            var deadlineAction = DeadlineAction.ExecuteCommand("test");
            var deadlineTime = DateTime.Now.AddHours(1);
            Action<string> logAction = (s) => { };
            Action<NotificationResult> resultAction = (r) => { };

            // Act
            options.Title = "Test Title";
            options.Message = "Test Message";
            options.LogoImagePath = "logo.png";
            options.HeroImagePath = "hero.png";
            options.Attribution = "Test Attribution";
            options.TimeoutInSeconds = 30;
            options.Buttons.Add(button);
            options.Async = true;
            options.Id = "test-id";
            options.Tag = "test-tag";
            options.Group = "test-group";
            options.DeferralOptions = deferralOptions;
            options.ShowReminder = true;
            options.ReminderTimeInMinutes = 15;
            options.PersistState = false;
            options.EnableLogging = true;
            options.LogAction = logAction;
            options.OnActivated = resultAction;
            options.OnTimeout = resultAction;
            options.OnError = resultAction;
            options.DeadlineTime = deadlineTime;
            options.DeadlineAction = deadlineAction;
            options.ShowCountdown = true;

            // Assert
            Assert.AreEqual("Test Title", options.Title);
            Assert.AreEqual("Test Message", options.Message);
            Assert.AreEqual("logo.png", options.LogoImagePath);
            Assert.AreEqual("hero.png", options.HeroImagePath);
            Assert.AreEqual("Test Attribution", options.Attribution);
            Assert.AreEqual(30, options.TimeoutInSeconds);
            Assert.AreEqual(1, options.Buttons.Count);
            Assert.AreEqual(button, options.Buttons[0]);
            Assert.IsTrue(options.Async);
            Assert.AreEqual("test-id", options.Id);
            Assert.AreEqual("test-tag", options.Tag);
            Assert.AreEqual("test-group", options.Group);
            Assert.AreEqual(deferralOptions, options.DeferralOptions);
            Assert.IsTrue(options.ShowReminder);
            Assert.AreEqual(15, options.ReminderTimeInMinutes);
            Assert.IsFalse(options.PersistState);
            Assert.IsTrue(options.EnableLogging);
            Assert.AreEqual(logAction, options.LogAction);
            Assert.AreEqual(resultAction, options.OnActivated);
            Assert.AreEqual(resultAction, options.OnTimeout);
            Assert.AreEqual(resultAction, options.OnError);
            Assert.AreEqual(deadlineTime, options.DeadlineTime);
            Assert.AreEqual(deadlineAction, options.DeadlineAction);
            Assert.IsTrue(options.ShowCountdown);
        }
    }
}
