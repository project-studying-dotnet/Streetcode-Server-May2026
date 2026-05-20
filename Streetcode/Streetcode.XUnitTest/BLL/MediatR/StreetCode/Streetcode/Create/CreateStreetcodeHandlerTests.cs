// <copyright file="CreateStreetcodeHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.Create
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutoMapper;
    using global::Streetcode.BLL.DTO.Streetcode;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode;
    using global::Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
    using global::Streetcode.DAL.Entities.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using global::Streetcode.DAL.Repositories.Interfaces.Streetcode;
    using Moq;
    using Xunit;

    /// <summary>
    /// Tests for CreateStreetcodeHandler, which handles the creation of a new streetcode entity in the system.
    /// </summary>
    public class CreateStreetcodeHandlerTests
    {
        private readonly IMapper mapper;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ILoggerService> loggerMock;

        private CreateStreetcodeHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreetcodeHandlerTests"/> class.
        /// </summary>
        public CreateStreetcodeHandlerTests()
        {
            this.mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<StreetcodeProfile>();
            }).CreateMapper();

            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.handler = new CreateStreetcodeHandler(
                    this.mapper,
                    this.repositoryWrapperMock.Object,
                    this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that when a new streetcode is created successfully, the handler returns a success result with the created streetcode data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Hahdler_WhenCreatedStreetcodeIsSuccessful_ReturnsSuccessResult()
        {
            // Arrange
            var newStreetcodeContent = new StreetcodeDTO { Title = "Test Streetcode" };

            var request = new CreateStreetcodeCommand(newStreetcodeContent);

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .CreateAsync(It.IsAny<StreetcodeContent>()))
                    .ReturnsAsync((StreetcodeContent sc) => sc);

            this.repositoryWrapperMock
                    .Setup(r => r.SaveChangesAsync())
                    .ReturnsAsync(1);

            // Act
            var result = await this.handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal("Test Streetcode", result.Value.Title);

            this.repositoryWrapperMock.Verify(
                    r => r.StreetcodeRepository.CreateAsync(
                        It.IsAny<StreetcodeContent>()),
                    Times.Once);

            this.repositoryWrapperMock.Verify(
                    r => r.SaveChangesAsync(),
                    Times.Once);
        }

        /// <summary>
        /// Tests that when the creation of a new streetcode fails, the handler returns a failure result with an appropriate error message and logs the error.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handler_WhenCreatedStreetcodeIsFailed_ReturnsFailResult()
        {
            // Arrange
            var newStreetcodeContent = new StreetcodeDTO { Title = "Test Streetcode" };

            var request = new CreateStreetcodeCommand(newStreetcodeContent);

            string expectedErrorMessage = "Failed to create a streetcode";

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .CreateAsync(It.IsAny<StreetcodeContent>()))
                    .ReturnsAsync((StreetcodeContent)null!);

            // Act
            var result = await this.handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            Assert.Equal(expectedErrorMessage, result.Errors[0].Message);

            this.repositoryWrapperMock.Verify(
                    r => r.StreetcodeRepository.CreateAsync(
                        It.IsAny<StreetcodeContent>()),
                    Times.Once);

            this.loggerMock.Verify(
                    l => l.LogError(request, expectedErrorMessage),
                    Times.Once);
        }

        /// <summary>
        /// Method tests that when the mapper returns null during the creation of a new streetcode, the handler returns a failure result with an appropriate error message and logs the error.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handler_WhenMapperReturnNull_ReturnFail()
        {
            // Arrange
            var request = new CreateStreetcodeCommand(null!);

            string expectedErrorMessage = "Cannot convert null to streetcode";

            // Act
            var result = await this.handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            Assert.Equal(expectedErrorMessage, result.Errors[0].Message);

            this.loggerMock.Verify(
                    l => l.LogError(request, expectedErrorMessage),
                    Times.Once);
        }
    }
}
