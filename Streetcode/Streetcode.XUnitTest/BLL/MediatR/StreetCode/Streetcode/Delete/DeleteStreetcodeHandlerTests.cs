// <copyright file="DeleteStreetcodeHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.Delete
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using AutoMapper;
    using FluentAssertions;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.MediatR.Streetcode.Streetcode.Delete;
    using global::Streetcode.DAL.Entities.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for DeleteStreetcodeHandler verifying not-found and successful-deletion behavior.
    /// </summary>
    public class DeleteStreetcodeHandlerTests
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly DeleteStreetcodeHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteStreetcodeHandlerTests"/> class.
        /// </summary>
        public DeleteStreetcodeHandlerTests()
        {
            this.mapperMock = new Mock<IMapper>();
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.handler = new DeleteStreetcodeHandler(
                this.mapperMock.Object,
                this.repositoryWrapperMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests the Handle method of the DeleteStreetcodeHandler returns false when the streetcode is not found.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_StreetcodeNotFound()
        {
            // Arrange
            int id = 1;

            var deleteStreetcodeCommand = new DeleteStreetcodeCommand(id);

            this.repositoryWrapperMock
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    null))
                .ReturnsAsync((StreetcodeContent)null!);

            // Act
            var result = await this.handler.Handle(deleteStreetcodeCommand, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            Assert.Equal($"No streetcode with such {id}", result.Errors[0].Message);

            this.loggerMock.Verify(
                logger => logger.LogError(deleteStreetcodeCommand, $"No streetcode with such {id}"),
                Times.Once);
        }

        /// <summary>
        /// Tests that the Handle method returns a successful result when the streetcode is found and deleted successfully.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnOk_StreetcodeDeleted()
        {
            // Arrange
            int id = 1;

            var deleteStreetcodeCommand = new DeleteStreetcodeCommand(id);

            var streetcodeContent = new StreetcodeContent { Id = id };

            this.repositoryWrapperMock
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    null))
                .ReturnsAsync(streetcodeContent);

            // Act
            var result = await this.handler.Handle(deleteStreetcodeCommand, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            this.repositoryWrapperMock.Verify(
                repo => repo.StreetcodeRepository.Delete(streetcodeContent),
                Times.Once);

            this.repositoryWrapperMock.Verify(
                repo => repo.SaveChanges(),
                Times.Once);
        }
    }
}
