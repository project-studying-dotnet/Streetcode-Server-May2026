// <copyright file="DeleteTextHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text.Delete
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentAssertions;
    using Moq;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.Mapping.Streetcode.TextContent;
    using Streetcode.BLL.MediatR.Streetcode.Text.Delete;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
    using Xunit;
    using T = Streetcode.DAL.Entities.Streetcode.TextContent;

    /// <summary>
    /// Unit tests for DeleteTextHandler.
    /// </summary>
    public class DeleteTextHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<ITextRepository> textRepoMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly DeleteTextHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTextHandlerTests"/> class.
        /// </summary>
        public DeleteTextHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TextProfile>();
            });

            this.mapper = config.CreateMapper();

            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.textRepoMock = new Mock<ITextRepository>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repoWrapperMock
                .Setup(x => x.TextRepository)
                .Returns(this.textRepoMock.Object);

            this.handler = new DeleteTextHandler(
                this.repoWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return successful Result when database delete succeeds.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnTextDto_WhenDeleteSucceeds()
        {
            int id = 1;
            var command = new DeleteTextCommand(id);
            var existingText = new T.Text { Id = id, Title = "Test Title" };

            this.textRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null))
                .ReturnsAsync(existingText);

            this.textRepoMock
                .Setup(r => r.Delete(It.IsAny<T.Text>()));

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            this.textRepoMock.Verify(
                r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null),
                Times.Once);
            this.textRepoMock.Verify(r => r.Delete(It.IsAny<T.Text>()), Times.Once);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Should return failed Result and log error when database save fails.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenDeleteFails()
        {
            int id = 1;
            var command = new DeleteTextCommand(id);
            var existingText = new T.Text { Id = id, Title = "Test Title" };
            string expectedErrorMsg = $"Failed to delete Text with Id {id}.";

            this.textRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null))
                .ReturnsAsync(existingText);

            this.textRepoMock
                .Setup(r => r.Delete(It.IsAny<T.Text>()));

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors
                .Should()
                .ContainSingle()
                .Which.Message
                .Should()
                .Be(expectedErrorMsg);

            this.textRepoMock.Verify(
                r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null),
                Times.Once);
            this.textRepoMock.Verify(r => r.Delete(It.IsAny<T.Text>()), Times.Once);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<DeleteTextCommand>(), expectedErrorMsg),
                Times.Once);
        }

        /// <summary>
        /// Should return failed Result when entity with given Id does not exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenTextDoesNotExist()
        {
            int id = 1;
            var command = new DeleteTextCommand(id);
            string expectedErrorMsg = $"Text with Id {id} not found.";

            this.textRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null))
                .ReturnsAsync((T.Text?)null);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors
                .Should()
                .ContainSingle()
                .Which.Message
                .Should()
                .Be(expectedErrorMsg);

            this.textRepoMock.Verify(
                r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null),
                Times.Once);
            this.textRepoMock.Verify(r => r.Delete(It.IsAny<T.Text>()), Times.Never);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<DeleteTextCommand>(), expectedErrorMsg),
                Times.Once);
        }

        /// <summary>
        /// Should propagate exception when repository throws.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldPropagateException_WhenRepositoryThrows()
        {
            var command = new DeleteTextCommand(1);

            this.textRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null))
                .Throws(new Exception("Database connection failure"));

            Func<Task> act = () => this.handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Database connection failure");

            this.textRepoMock.Verify(
                r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Text, bool>>>(), null),
                Times.Once);
            this.textRepoMock.Verify(r => r.Delete(It.IsAny<T.Text>()), Times.Never);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}