using FluentAssertions;
using Streetcode.EmailService.Contracts;
using Streetcode.XUnitTest.EmailService.Constants;
using Xunit;

namespace Streetcode.XUnitTest.EmailService.Contracts;

public class EmailMessageContractTests
{
    [Fact]
    public void Should_Set_Properties_Correctly()
    {
        var contract = new EmailMessageContract
        {
            To = [EmailTestConstants.TestEmail],
            From = EmailTestConstants.FromEmail,
            Subject = EmailTestConstants.Subject,
            Content = EmailTestConstants.Content,
        };

        contract.To.Should().ContainSingle().Which.Should().Be(EmailTestConstants.TestEmail);
        contract.From.Should().Be(EmailTestConstants.FromEmail);
        contract.Subject.Should().Be(EmailTestConstants.Subject);
        contract.Content.Should().Be(EmailTestConstants.Content);
    }
}