// <copyright file="GetParsedTextAdminPreviewHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text
{
    using FluentAssertions;
    using Moq;
    using Streetcode.BLL.Interfaces.Text;
    using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="GetParsedTextAdminPreviewHandler"/>.
    /// </summary>
    public class GetParsedTextAdminPreviewHandlerTests
    {
        private readonly Mock<ITextService> textServiceMock;
        private readonly GetParsedTextAdminPreviewHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParsedTextAdminPreviewHandlerTests"/> class.
        /// </summary>
        public GetParsedTextAdminPreviewHandlerTests()
        {
            this.textServiceMock = new Mock<ITextService>();
            this.handler = new GetParsedTextAdminPreviewHandler(this.textServiceMock.Object);
        }

        /// <summary>
        /// Tests that the Handle method returns an error when the text service returns null, indicating that parsing was unsuccessful.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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

        /// <summary>
        /// Tests that the Handle method returns the processed text when the text service successfully parses the input text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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

        /// <summary>
        /// Tests that the Handle method calls the text service with the original input text, ensuring that the correct data is passed for processing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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
}