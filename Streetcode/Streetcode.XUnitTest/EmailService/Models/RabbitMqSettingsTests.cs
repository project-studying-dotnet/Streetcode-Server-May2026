using FluentAssertions;
using Streetcode.EmailService.Models;
using Streetcode.XUnitTest.EmailService.Constants;
using Xunit;

namespace Streetcode.XUnitTest.EmailService.Models;

public class RabbitMqSettingsTests
{
    [Fact]
    public void Should_Set_Properties_Correctly()
    {
        var settings = new RabbitMqSettings
        {
            HostName = EmailTestConstants.RabbitMqHostName,
            Port = EmailTestConstants.RabbitMqPort,
            UserName = EmailTestConstants.RabbitMqUserName,
            Password = EmailTestConstants.RabbitMqPassword,
            QueueName = EmailTestConstants.RabbitMqQueueName,
        };

        settings.HostName.Should().Be(EmailTestConstants.RabbitMqHostName);
        settings.Port.Should().Be(EmailTestConstants.RabbitMqPort);
        settings.UserName.Should().Be(EmailTestConstants.RabbitMqUserName);
        settings.Password.Should().Be(EmailTestConstants.RabbitMqPassword);
        settings.QueueName.Should().Be(EmailTestConstants.RabbitMqQueueName);
    }
}