// <copyright file="GetParsedTextAdminPreviewHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using FluentAssertions;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Moq;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text
{

    /// <summary>
    /// Unit tests for <see cref="GetParsedTextAdminPreviewHandler"/>.
    /// </summary>
    public class GetParsedTextAdminPreviewHandlerTests
    {
        private readonly Mock<ITextService> _textServiceMock;
        private readonly GetParsedTextAdminPreviewHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParsedTextAdminPreviewHandlerTests"/> class.
        /// </summary>
        public GetParsedTextAdminPreviewHandlerTests()
        {
            _textServiceMock = new Mock<ITextService>();
            _handler = new GetParsedTextAdminPreviewHandler(_textServiceMock.Object);
        }

        /// <summary>
        /// Tests that the Handle method returns an error when the text service returns null, indicating that parsing was unsuccessful.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsParsedText_WhenServiceReturnsText()
        {
            var command = new GetParsedTextForAdminPreviewCommand("raw text");
            const string parsedText = "parsed text";

            _textServiceMock
                .Setup(s => s.AddTermsTag(command.textToParse))
                .ReturnsAsync(parsedText);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(parsedText);
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

            _textServiceMock
                .Setup(s => s.AddTermsTag(command.textToParse))
                .ReturnsAsync(parsedText);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

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

            _textServiceMock
                .Setup(s => s.AddTermsTag(originalText))
                .ReturnsAsync("processed");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _textServiceMock.Verify(
                s => s.AddTermsTag(originalText),
                Times.Once);
        }
    }
}
