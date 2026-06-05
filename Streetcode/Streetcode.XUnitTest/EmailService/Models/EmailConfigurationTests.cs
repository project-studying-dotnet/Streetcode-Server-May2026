using FluentAssertions;
using Xunit;
using Streetcode.EmailService.Models;
using Streetcode.XUnitTest.EmailService.Constants;

namespace Streetcode.EmailService.XUnitTest.Models;

public class EmailConfigurationTests
{
    [Fact]
    public void Should_Set_Properties_Correctly()
    {
        var config = new EmailConfiguration
        {
            From = EmailTestConstants.FromEmail,
            SmtpServer = EmailTestConstants.SmtpServer,
            Port = EmailTestConstants.Port,
            UserName = EmailTestConstants.UserName,
            Password = EmailTestConstants.Password,
        };

        config.From.Should().Be(EmailTestConstants.FromEmail);
        config.SmtpServer.Should().Be(EmailTestConstants.SmtpServer);
        config.Port.Should().Be(EmailTestConstants.Port);
        config.UserName.Should().Be(EmailTestConstants.UserName);
        config.Password.Should().Be(EmailTestConstants.Password);
    }
}