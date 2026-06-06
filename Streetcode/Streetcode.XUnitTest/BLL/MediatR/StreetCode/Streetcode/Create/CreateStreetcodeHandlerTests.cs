// <copyright file="CreateStreetcodeHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.Create
{
    /// <summary>
    /// Tests for CreateStreetcodeHandler, which handles the creation of a new streetcode entity in the system.
    /// </summary>
    public class CreateStreetcodeHandlerTests
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ILoggerService> loggerMock;

        private CreateStreetcodeHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreetcodeHandlerTests"/> class.
        /// </summary>
        public CreateStreetcodeHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.loggerMock = new Mock<ILoggerService>();
            this.mapperMock = new Mock<IMapper>();

            this.handler = new CreateStreetcodeHandler(
                    this.mapperMock.Object,
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
            var dto = new StreetcodeDTO
            {
                Id = 1,
                Title = "Test Streetcode",
                Tags = new List<StreetcodeTagDTO>
                {
                    new StreetcodeTagDTO
                    {
                        Id = 1,
                        Title = "Test Tag"
                    }
                }
            };

            var streetcode = new StreetcodeContent
            {
                Id = 1,
                Title = "Test Streetcode",
                StreetcodeTagIndices = new List<StreetcodeTagIndex>
                {
                    new StreetcodeTagIndex
                    {
                        TagId = 1,
                        StreetcodeId = 1
                    }
                }
            };

            var tag = new Tag
            {
                Id = 1,
                Title = "Test Tag"
            };

            var request = new CreateStreetcodeCommand(dto);

            this.mapperMock
                    .Setup(m => m.Map<StreetcodeContent>(It.IsAny<StreetcodeDTO>()))
                    .Returns(streetcode);

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .GetAllAsync(
                            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                            It.IsAny<Func<IQueryable<StreetcodeContent>,
                                IIncludableQueryable<StreetcodeContent, object>>>()))
                    .ReturnsAsync(new List<StreetcodeContent>());

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .CreateAsync(It.IsAny<StreetcodeContent>()))
                    .ReturnsAsync(streetcode);

            this.repositoryWrapperMock
                    .Setup(r => r.SaveChangesAsync())
                    .ReturnsAsync(1);

            this.repositoryWrapperMock
                    .Setup(r => r.TagRepository
                        .GetAllAsync(
                            It.IsAny<Expression<Func<Tag, bool>>>(),
                            It.IsAny<Func<IQueryable<Tag>,
                                IIncludableQueryable<Tag, object>>>()))
                    .ReturnsAsync(new List<Tag>());

            this.mapperMock
                .Setup(m => m.Map<StreetcodeDTO>(It.IsAny<StreetcodeContent>()))
                .Returns(dto);

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
                    Times.Exactly(2));
        }

        /// <summary>
        /// Tests that when the creation of a new streetcode fails, the handler returns a failure result with an appropriate error message and logs the error.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handler_WhenCreatedStreetcodeIsFailed_ReturnsFailResult()
        {
            // Arrange
            var streetcode = new StreetcodeContent() { Title = "Test Streetcode" };

            var newStreetcodeContent = new StreetcodeDTO
            {
                Id = 1,
                Title = "Test Streetcode",
                Tags = new List<StreetcodeTagDTO>
                {
                    new StreetcodeTagDTO { Id = 1, Title = "Test Tag" }
                }
            };

            var request = new CreateStreetcodeCommand(newStreetcodeContent);

            string expectedErrorMessage = "Test exception";

            this.mapperMock
                    .Setup(m => m.Map<StreetcodeContent>(It.IsAny<StreetcodeDTO>()))
                    .Returns(streetcode);

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .CreateAsync(It.IsAny<StreetcodeContent>()))
                    .Throws(new Exception(expectedErrorMessage));

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
