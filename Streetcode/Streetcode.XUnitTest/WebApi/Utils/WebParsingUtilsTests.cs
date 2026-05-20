// <copyright file="WebParsingUtilsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.WebApi.Utils
{
    using FluentAssertions;
    using Streetcode.WebApi.Utils;
    using Xunit;

    /// <summary>
    /// Contains tests for <see cref="WebParsingUtils"/>.
    /// </summary>
    public class WebParsingUtilsTests
    {
        private const string ValidUrl = "https://example.com/file.zip";
        private static readonly string ValidZipPath = Path.Combine(Path.GetTempPath(), "houses.zip");
        private static readonly string ValidExtractTo = Path.GetTempPath();

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="fileUrl"/> is null.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenFileUrlIsNull()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                null!, ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("fileUrl");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="fileUrl"/> is empty.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenFileUrlIsEmpty()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                string.Empty, ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("fileUrl");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="fileUrl"/> is not a valid URI.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenFileUrlIsInvalidUri()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                "not-a-valid-url", ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("fileUrl");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="zipPath"/> is null.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenZipPathIsNull()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, null!, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("zipPath");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="zipPath"/> is empty.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenZipPathIsEmpty()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, string.Empty, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("zipPath");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="extractTo"/> is null.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenExtractToIsNull()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, ValidZipPath, null!, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("extractTo");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="extractTo"/> is empty.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenExtractToIsEmpty()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, ValidZipPath, string.Empty, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("extractTo");
        }

        /// <summary>
        /// Should not throw <see cref="ArgumentException"/> when all arguments are valid absolute paths.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldNotThrowArgumentException_WhenAllArgumentsAreValid()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().NotThrowAsync<ArgumentException>();
        }
    }
}