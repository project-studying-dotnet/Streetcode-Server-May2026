// <copyright file="UpdateStreetcodeHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.Update
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutoMapper;
    using global::Streetcode.BLL.DTO.Streetcode;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode;
    using global::Streetcode.BLL.MediatR.Streetcode.Streetcode.Update;
    using global::Streetcode.DAL.Entities.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using Moq;
    using Xunit;

    /// <summary>
    /// Test for UpdateStreetcodeHandler, which handles the updating of an existing streetcode entity in the system.
    /// </summary>
    public class UpdateStreetcodeHandlerTests
    {
        private readonly IMapper mapper;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ILoggerService> loggerMock;

        private UpdateStreetcodeHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStreetcodeHandlerTests"/> class.
        /// </summary>
        public UpdateStreetcodeHandlerTests()
        {
            this.mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<StreetcodeProfile>();
            }).CreateMapper();

            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.handler = new UpdateStreetcodeHandler(
                    this.mapper,
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
            };

            var streetcodeEntity = this.mapper.Map<StreetcodeContent>(streetcodeDto);

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository.Update(It.IsAny<StreetcodeContent>()));

            this.repositoryWrapperMock
                    .Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var command = new UpdateStreetcodeCommand(streetcodeDto);

            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal(streetcodeDto.Id, result.Value.Id);

            Assert.Equal(streetcodeDto.Title, result.Value.Title);

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
            };

            string expectedErrorMessage = $"Failed to update streetcode with ID {streetcodeDto.Id}";

            var streetcodeEntity = this.mapper.Map<StreetcodeContent>(streetcodeDto);

            this.repositoryWrapperMock
                    .Setup(r => r.StreetcodeRepository
                        .Update(It.IsAny<StreetcodeContent>()));

            this.repositoryWrapperMock
                    .Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

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
            var request = new UpdateStreetcodeCommand(null);

            string expextedErrorMessage = $"Cannot convert null to streetcode";

            // Act
            var result = await this.handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            Assert.Equal(expextedErrorMessage, result.Errors[0].Message);

            this.loggerMock.Verify(
                   l => l.LogError(request, expextedErrorMessage),
                   Times.Once);
        }
    }
}
