// <copyright file="UpdateStreetcodeHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.Update
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using AutoMapper;
    using global::Streetcode.BLL.DTO.AdditionalContent.Tag;
    using global::Streetcode.BLL.DTO.Streetcode;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode;
    using global::Streetcode.BLL.MediatR.Streetcode.Streetcode.Update;
    using global::Streetcode.DAL.Entities.AdditionalContent;
    using global::Streetcode.DAL.Entities.Media.Images;
    using global::Streetcode.DAL.Entities.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Xunit;

    /// <summary>
    /// Test for UpdateStreetcodeHandler, which handles the updating of an existing streetcode entity in the system.
    /// </summary>
    public class UpdateStreetcodeHandlerTests
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ILoggerService> loggerMock;

        private UpdateStreetcodeHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStreetcodeHandlerTests"/> class.
        /// </summary>
        public UpdateStreetcodeHandlerTests()
        {
            this.mapperMock = new Mock<IMapper>();
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.handler = new UpdateStreetcodeHandler(
                    this.mapperMock.Object,
                    this.repositoryWrapperMock.Object,
                    this.loggerMock.Object);
        }

        /// <summary>
        /// Tests the Handle method of the UpdateStreetcodeHandler when the streetcode is successfully updated.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_UpdatedStreetcodeIsSuccess_ReturnsOkResult()
        {
            // Arrange
            var streetcodeDto = new StreetcodeDTO
            {
                Id = 1,
                Title = "Updated Streetcode",
                Tags = new List<StreetcodeTagDTO>()
                {
                    new StreetcodeTagDTO() { Id = 1, Title = "Test Tag" }
                }
            };

            var streetcode = new StreetcodeContent
            {
                Id = 1,
                Title = "Updated Streetcode",
                Tags = new List<Tag>()
                {
                    new Tag() { Id = 1, Title = "Test Tag" }
                }
            };

            this.mapperMock
                .Setup(m => m.Map<StreetcodeContent>(It.IsAny<StreetcodeDTO>()))
                .Returns(streetcode);

            this.repositoryWrapperMock
                   .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                       It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                       It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                   .ReturnsAsync(streetcode);

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository.Update(It.IsAny<StreetcodeContent>()));

            this.repositoryWrapperMock
                    .Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            this.repositoryWrapperMock
                .Setup(r => r.StreetcodeTagIndexRepository.GetAllAsync(
                    It.IsAny<Expression<Func<StreetcodeTagIndex, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeTagIndex>, 
                        IIncludableQueryable<StreetcodeTagIndex, object>>>()))
                .ReturnsAsync(new List<StreetcodeTagIndex>());

            this.mapperMock
                .Setup(m => m.Map<StreetcodeDTO>(It.IsAny<StreetcodeContent>()))
                .Returns(streetcodeDto);

            var command = new UpdateStreetcodeCommand(streetcodeDto);

            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal(streetcodeDto.Id, result.Value.Id);

            Assert.Equal(streetcodeDto.Title, result.Value.Title);

            Assert.Equal(streetcodeDto.Tags.First().Id, result.Value.Tags.First().Id);

            this.repositoryWrapperMock.Verify(
                r => r.StreetcodeRepository
                    .Update(It.IsAny<StreetcodeContent>()), Times.Once);
        }

        /// <summary>
        /// Tests the Handle method of the UpdateStreetcodeHandler when the streetcode update operation fails, ensuring that it returns a failure result and logs an appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_UpdatedStreetcodeIsFailed_ReturnsFailResult()
        {
            // Arrange
            var streetcodeDto = new StreetcodeDTO
            {
                Id = 1,
                Title = "Updated Streetcode",
                Tags = new List<StreetcodeTagDTO>()
                {
                    new StreetcodeTagDTO() { Id = 1, Title = "Test Tag" }
                }
            };

            var streetcode = new StreetcodeContent
            {
                Id = 1,
                Title = "Updated Streetcode",
            };

            this.mapperMock
                .Setup(m => m.Map<StreetcodeContent>(It.IsAny<StreetcodeDTO>()))
                .Returns(streetcode);

            string expectedErrorMessage = $"Failed to update streetcode with ID {streetcodeDto.Id}";

            this.repositoryWrapperMock
                   .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                       It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                       It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                   .ReturnsAsync(streetcode);

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .Update(It.IsAny<StreetcodeContent>()))
                    .Throws(new Exception(expectedErrorMessage));

            var request = new UpdateStreetcodeCommand(streetcodeDto);

            // Act
            var result = await this.handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);

            Assert.Equal(expectedErrorMessage, result.Errors[0].Message);

            this.repositoryWrapperMock.Verify(
                    r => r.StreetcodeRepository
                        .Update(It.IsAny<StreetcodeContent>()), Times.Once);

            this.loggerMock.Verify(
                    l => l.LogError(request, expectedErrorMessage),
                    Times.Once);
        }

        /// <summary>
        /// Tests the Handle method of the UpdateStreetcodeHandler when the mapper returns null, ensuring that it returns a failure result and logs an appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_MapperResultNull_ReturnsFailResult()
        {
            // Arrange
            var request = new UpdateStreetcodeCommand(null!);

            string expextedErrorMessage = $"Cannot convert null to streetcode";

            var streetcode = new StreetcodeContent
            {
                Id = 1,
                Title = "Updated Streetcode",
            };

            this.repositoryWrapperMock
                   .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                       It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                       It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                   .ReturnsAsync(streetcode);

            // Act
            var result = await this.handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            Assert.Equal(expextedErrorMessage, result.Errors[0].Message);

            this.loggerMock.Verify(
                   l => l.LogError(request, expextedErrorMessage),
                   Times.Once);
        }

        /// <summary>
        /// Verifies that handling an update for a non-existent streetcode returns a failed result and logs the expected
        /// error.
        /// </summary>
        /// <remarks>Asserts that the result is failed, the first error message indicates the missing
        /// streetcode ID, and an error log entry is written exactly once.</remarks>
        /// <returns>A Task representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_StreetcodeNotFound_ReturnsFailResult()
        {
            // Arrange
            var streetcodeDto = new StreetcodeDTO
            {
                Id = 1,
                Title = "Updated Streetcode",
            };

            string expectedErrorMessage = $"Streetcode with ID {streetcodeDto.Id} not found";

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .GetFirstOrDefaultAsync(
                            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                            It.IsAny<Func<IQueryable<StreetcodeContent>,
                                IIncludableQueryable<StreetcodeContent, object>>>()))
                    .ReturnsAsync((StreetcodeContent)null!);

            var request = new UpdateStreetcodeCommand(streetcodeDto);

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
