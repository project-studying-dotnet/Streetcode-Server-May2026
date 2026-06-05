using FluentAssertions;
using Xunit;
using Streetcode.EmailService.Models;
using Streetcode.XUnitTest.EmailService.Constants;

namespace Streetcode.XUnitTest.EmailService.Models;

public class MessageTests
{
    [Fact]
    public void Should_Set_Properties_Correctly()
    {
        var message = new Message
        {
            To = [EmailTestConstants.TestEmail],
            From = EmailTestConstants.FromEmail,
            Subject = EmailTestConstants.Subject,
            Content = EmailTestConstants.Content,
        };

        message.To.Should().ContainSingle().Which.Should().Be(EmailTestConstants.TestEmail);
        message.From.Should().Be(EmailTestConstants.FromEmail);
        message.Subject.Should().Be(EmailTestConstants.Subject);
        message.Content.Should().Be(EmailTestConstants.Content);
    }
}