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
}
