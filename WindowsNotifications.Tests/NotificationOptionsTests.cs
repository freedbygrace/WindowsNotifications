using NUnit.Framework;
using System;
using System.Linq;
using WindowsNotifications.Models;

namespace WindowsNotifications.Tests
{
    [TestFixture]
    public class NotificationOptionsTests
    {
        [Test]
        public void Constructor_SetsDefaultValues()
        {
            // Act
            var options = new NotificationOptions();

            // Assert
            Assert.IsNull(options.Title);
            Assert.IsNull(options.Message);
            Assert.IsNull(options.LogoImagePath);
            Assert.IsNull(options.HeroImagePath);
            Assert.IsNull(options.Attribution);
            Assert.AreEqual(0, options.TimeoutInSeconds);
            Assert.IsFalse(options.Async);
            Assert.IsNotNull(options.Id);
            Assert.IsTrue(Guid.TryParse(options.Id, out _));
            Assert.IsNull(options.Tag);
            Assert.IsNull(options.Group);
            Assert.IsFalse(options.PersistState);
            Assert.IsTrue(options.EnableLogging);
            Assert.IsNull(options.LogAction);
            Assert.IsNotNull(options.Buttons);
            Assert.AreEqual(0, options.Buttons.Count);
        }

        [Test]
        public void Properties_CanBeSet()
        {
            // Arrange
            var options = new NotificationOptions();
            Action<string> logAction = (log) => { };

            // Act
            options.Title = "Test Title";
            options.Message = "Test Message";
            options.LogoImagePath = "logo.png";
            options.HeroImagePath = "hero.png";
            options.Attribution = "Test Attribution";
            options.TimeoutInSeconds = 30;
            options.Async = true;
            options.Id = "custom-id";
            options.Tag = "test-tag";
            options.Group = "test-group";
            options.PersistState = true;
            options.EnableLogging = false;
            options.LogAction = logAction;

            // Assert
            Assert.AreEqual("Test Title", options.Title);
            Assert.AreEqual("Test Message", options.Message);
            Assert.AreEqual("logo.png", options.LogoImagePath);
            Assert.AreEqual("hero.png", options.HeroImagePath);
            Assert.AreEqual("Test Attribution", options.Attribution);
            Assert.AreEqual(30, options.TimeoutInSeconds);
            Assert.IsTrue(options.Async);
            Assert.AreEqual("custom-id", options.Id);
            Assert.AreEqual("test-tag", options.Tag);
            Assert.AreEqual("test-group", options.Group);
            Assert.IsTrue(options.PersistState);
            Assert.IsFalse(options.EnableLogging);
            Assert.AreEqual(logAction, options.LogAction);
        }

        [Test]
        public void Buttons_CanBeAdded()
        {
            // Arrange
            var options = new NotificationOptions();
            var button1 = new NotificationButton("Button 1");
            var button2 = new NotificationButton("Button 2", "custom-id", "custom-arg");

            // Act
            options.Buttons.Add(button1);
            options.Buttons.Add(button2);

            // Assert
            Assert.AreEqual(2, options.Buttons.Count);
            Assert.AreEqual("Button 1", options.Buttons[0].Text);
            Assert.AreEqual("Button 2", options.Buttons[1].Text);
            Assert.AreEqual("custom-id", options.Buttons[1].Id);
            Assert.AreEqual("custom-arg", options.Buttons[1].Argument);
        }
    }

    [TestFixture]
    public class NotificationButtonTests
    {
        [Test]
        public void Constructor_SetsText()
        {
            // Arrange & Act
            var button = new NotificationButton("Button Text");

            // Assert
            Assert.AreEqual("Button Text", button.Text);
            Assert.IsNotNull(button.Id);
            Assert.IsTrue(Guid.TryParse(button.Id, out _));
            Assert.AreEqual(button.Id, button.Argument);
        }

        [Test]
        public void Constructor_SetsCustomId()
        {
            // Arrange & Act
            var button = new NotificationButton("Button Text", "custom-id");

            // Assert
            Assert.AreEqual("Button Text", button.Text);
            Assert.AreEqual("custom-id", button.Id);
            Assert.AreEqual("custom-id", button.Argument);
        }

        [Test]
        public void Constructor_SetsCustomArgument()
        {
            // Arrange & Act
            var button = new NotificationButton("Button Text", "custom-id", "custom-arg");

            // Assert
            Assert.AreEqual("Button Text", button.Text);
            Assert.AreEqual("custom-id", button.Id);
            Assert.AreEqual("custom-arg", button.Argument);
        }

        [Test]
        public void Properties_CanBeSet()
        {
            // Arrange
            var button = new NotificationButton("Initial Text");

            // Act
            button.Text = "Updated Text";
            button.Id = "updated-id";
            button.Argument = "updated-arg";

            // Assert
            Assert.AreEqual("Updated Text", button.Text);
            Assert.AreEqual("updated-id", button.Id);
            Assert.AreEqual("updated-arg", button.Argument);
        }
    }
}
