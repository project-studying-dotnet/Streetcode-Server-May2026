using FluentAssertions;
using Streetcode.Auth.Models;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.Models
{
    public class RabbitMqSettingsTests
    {
        [Fact]
        public void RabbitMqSettings_ShouldStoreValuesCorrectly()
        {
            // Arrange
            var settings = new RabbitMqSettings
            {
                Host = "localhost",
                VirtualHost = "test-vh",
                UserName = "guest",
                Password = "password123"
            };

            // Act & Assert
            settings.Host.Should().Be("localhost");
            settings.VirtualHost.Should().Be("test-vh");
            settings.UserName.Should().Be("guest");
            settings.Password.Should().Be("password123");
        }

        [Fact]
        public void RabbitMqSettings_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var settings = new RabbitMqSettings();

            // Assert
            settings.Host.Should().Be(string.Empty);
            settings.VirtualHost.Should().Be("/");
            settings.UserName.Should().Be(string.Empty);
            settings.Password.Should().Be(string.Empty);
        }
    }
}