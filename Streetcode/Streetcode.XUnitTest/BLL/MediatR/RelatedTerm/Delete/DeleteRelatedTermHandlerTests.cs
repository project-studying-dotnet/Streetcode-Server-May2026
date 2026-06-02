using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using Moq;
using FluentAssertions;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Streetcode.BLL.Resources;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTerm.Delete
{
    public class DeleteRelatedTermHandlerTests
    {
        private const string TestWord = "test";
        private const int TermId = 1;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IRelatedTermRepository> _relatedTermRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly DeleteRelatedTermHandler _handler;

        public DeleteRelatedTermHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _relatedTermRepositoryMock = new Mock<IRelatedTermRepository>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.RelatedTermRepository)
                .Returns(_relatedTermRepositoryMock.Object);
            _handler = new DeleteRelatedTermHandler(
                    _repositoryWrapperMock.Object,
                    _mapperMock.Object,
                    _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRelatedTermNotFound()
        {
            var command = new DeleteRelatedTermCommand(TestWord, TermId);

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    null))
                .ReturnsAsync((Entity)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.CannotFindRelatedTerm, TestWord));

            _loggerMock.Verify(
                logger => logger.LogError(command, string.Format(ErrorMessages.CannotFindRelatedTerm, TestWord)),
                Times.Once);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Delete(It.IsAny<Entity>()),
                Times.Never);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Never);

            _mapperMock.Verify(
                mapper => mapper.Map<RelatedTermDTO>(It.IsAny<Entity>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            var command = new DeleteRelatedTermCommand(TestWord, TermId);
            var entity = new Entity { Id = 1, TermId = 1, Word = "test" };
            var dto = new RelatedTermDTO { Id = 1, TermId = 1, Word = "test" };

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    null))
                .ReturnsAsync(entity);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(0);

            _mapperMock
                .Setup(mapper => mapper.Map<RelatedTermDTO>(entity))
                .Returns(dto);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.FailedToDeleteRelatedTerm);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Delete(entity),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(command, ErrorMessages.FailedToDeleteRelatedTerm),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMappingToDtoFails()
        {
            var command = new DeleteRelatedTermCommand(TestWord, TermId);
            var entity = new Entity { Id = 1, TermId = 1, Word = TestWord };

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    null))
                .ReturnsAsync(entity);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(mapper => mapper.Map<RelatedTermDTO>(entity))
                .Returns((RelatedTermDTO)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.FailedToDeleteRelatedTerm);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Delete(entity),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(command, ErrorMessages.FailedToDeleteRelatedTerm),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenRelatedTermDeletedSuccessfully()
        {
            var command = new DeleteRelatedTermCommand(TestWord, TermId);
            var entity = new Entity { Id = 1, TermId = 1, Word = TestWord };
            var dto = new RelatedTermDTO { Id = 1, TermId = 1, Word = TestWord };

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    null))
                .ReturnsAsync(entity);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(mapper => mapper.Map<RelatedTermDTO>(entity))
                .Returns(dto);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dto);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Delete(entity),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Once);

            _mapperMock.Verify(
                mapper => mapper.Map<RelatedTermDTO>(entity),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}