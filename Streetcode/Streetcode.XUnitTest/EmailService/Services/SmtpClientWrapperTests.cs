using FluentAssertions;
using Streetcode.EmailService.Services;
using Xunit;

namespace Streetcode.XUnitTest.EmailService.Services;

public class SmtpClientWrapperTests
{
    [Fact]
    public async Task DisposeAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var wrapper = new SmtpClientWrapper();

        // Act
        var act = async () => await wrapper.DisposeAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void IsConnected_ShouldBeFalse_WhenClientIsNotConnected()
    {
        // Arrange
        var wrapper = new SmtpClientWrapper();

        // Act
        var result = wrapper.IsConnected;

        // Assert
        result.Should().BeFalse();
    }
}