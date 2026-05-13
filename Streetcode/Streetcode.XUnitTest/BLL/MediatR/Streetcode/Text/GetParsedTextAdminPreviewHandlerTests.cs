using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.Text;

public class GetParsedTextAdminPreviewHandlerTests
{
    private readonly Mock<ITextService> textServiceMock;
    private readonly GetParsedTextAdminPreviewHandler handler;

    public GetParsedTextAdminPreviewHandlerTests()
    {
        this.textServiceMock = new Mock<ITextService>();
        this.handler = new GetParsedTextAdminPreviewHandler(this.textServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenServiceReturnsNull()
    {
        // Arrange
        var command = new GetParsedTextForAdminPreviewCommand(textToParse: "some text");

        this.textServiceMock
            .Setup(s => s.AddTermsTag(command.textToParse))
            .ReturnsAsync((string?)null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "text was not parsed successfully");
    }

    [Fact]
    public async Task Handle_ReturnsProcessedText_WhenServiceSucceeds()
    {
        // Arrange
        const string parsedText = "<Popover><Term>Майдан</Term><Desc>Центральна площа</Desc></Popover>";
        var command = new GetParsedTextForAdminPreviewCommand(textToParse: "some text");

        this.textServiceMock
            .Setup(s => s.AddTermsTag(command.textToParse))
            .ReturnsAsync(parsedText);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(parsedText);
    }

    [Fact]
    public async Task Handle_PassesOriginalText_ToService()
    {
        // Arrange
        const string originalText = "raw input text";
        var command = new GetParsedTextForAdminPreviewCommand(textToParse: originalText);

        this.textServiceMock
            .Setup(s => s.AddTermsTag(originalText))
            .ReturnsAsync("processed");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.textServiceMock.Verify(
            s => s.AddTermsTag(originalText),
            Times.Once);
    }

}
