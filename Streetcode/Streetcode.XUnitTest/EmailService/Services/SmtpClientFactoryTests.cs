using FluentAssertions;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Services;
using Xunit;

namespace Streetcode.XUnitTest.EmailService.Services;

public class SmtpClientFactoryTests
{
    [Fact]
    public void CreateClient_ShouldReturnSmtpClientWrapper()
    {
        // Arrange
        var factory = new SmtpClientFactory();

        // Act
        var client = factory.CreateClient();

        // Assert
        client.Should().BeAssignableTo<ISmtpClientWrapper>();
    }
}