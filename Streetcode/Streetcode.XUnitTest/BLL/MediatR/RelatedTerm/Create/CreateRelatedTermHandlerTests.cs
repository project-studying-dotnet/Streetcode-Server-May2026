using AutoMapper;
using FluentResults;
using Moq;
using FluentAssertions;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTerm.Create
{
    public class CreateRelatedTermHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IRelatedTermRepository> _relatedTermRepositoryMock;
        private readonly CreateRelatedTermHandler _handler;

        public CreateRelatedTermHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _relatedTermRepositoryMock = new Mock<IRelatedTermRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.RelatedTermRepository)
                .Returns(_relatedTermRepositoryMock.Object);

            _handler = new CreateRelatedTermHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMappingToEntityFails()
        {
            var dto = new RelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);

            _mapperMock
                .Setup(mapper => mapper.Map<Entity>(dto))
                .Returns((Entity)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Cannot create new related word for a term!");

            _loggerMock.Verify(
                logger => logger.LogError(command, "Cannot create new related word for a term!"),
                Times.Once);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Create(It.IsAny<Entity>()),
                Times.Never);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRelatedTermAlreadyExists()
        {
            var dto = new RelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);
            var entity = new Entity { TermId = 1, Word = "test" };

            _mapperMock
                .Setup(mapper => mapper.Map<Entity>(dto))
                .Returns(entity);

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync(new List<Entity> { entity });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Слово з цим визначенням уже існує");

            _loggerMock.Verify(
                logger => logger.LogError(command, "Слово з цим визначенням уже існує"),
                Times.Once);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Create(It.IsAny<Entity>()),
                Times.Never);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenExistingTermsIsNull()
        {
            var dto = new RelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);
            var entity = new Entity { TermId = 1, Word = "test" };

            _mapperMock
                .Setup(mapper => mapper.Map<Entity>(dto))
                .Returns(entity);

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync((IEnumerable<Entity>)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Слово з цим визначенням уже існує");

            _loggerMock.Verify(
                logger => logger.LogError(command, "Слово з цим визначенням уже існує"),
                Times.Once);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Create(It.IsAny<Entity>()),
                Times.Never);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            var dto = new RelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);
            var entity = new Entity { TermId = 1, Word = "test" };

            _mapperMock
                .Setup(mapper => mapper.Map<Entity>(It.IsAny<RelatedTermDTO>()))
                .Returns(entity);

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync(new List<Entity>());

            _relatedTermRepositoryMock
                .Setup(repo => repo.Create(It.IsAny<Entity>()))
                .Returns((Entity relatedTerm) => relatedTerm);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should()
                .Be("Cannot save changes in the database after related word creation!");

            _relatedTermRepositoryMock.Verify(
                repo => repo.Create(It.IsAny<Entity>()),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(command,
                "Cannot save changes in the database after related word creation!"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMappingToDtoFails()
        {
            var dto = new RelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);
            var entity = new Entity { TermId = 1, Word = "test" };

            _mapperMock
                .Setup(mapper => mapper.Map<Entity>(It.IsAny<RelatedTermDTO>()))
                .Returns(entity);

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync(new List<Entity>());

            _relatedTermRepositoryMock
                .Setup(repo => repo.Create(It.IsAny<Entity>()))
                .Returns((Entity relatedTerm) => relatedTerm);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(mapper => mapper.Map<RelatedTermDTO>(It.IsAny<Entity>()))
                .Returns((RelatedTermDTO)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Cannot map entity!");

            _loggerMock.Verify(
                logger => logger.LogError(command, "Cannot map entity!"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenRelatedTermCreatedSuccessfully()
        {
            var dto = new RelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);
            var entity = new Entity { TermId = 1, Word = "test" };
            var createdDto = new RelatedTermDTO { TermId = 1, Word = "test" };

            _mapperMock
                .Setup(mapper => mapper.Map<Entity>(It.IsAny<RelatedTermDTO>()))
                .Returns(entity);

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync(new List<Entity>());

            _relatedTermRepositoryMock
                .Setup(repo => repo.Create(It.IsAny<Entity>()))
                .Returns((Entity relatedTerm) => relatedTerm);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(mapper => mapper.Map<RelatedTermDTO>(It.IsAny<Entity>()))
                .Returns(createdDto);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(createdDto);

            _relatedTermRepositoryMock.Verify(
                repo => repo.Create(It.IsAny<Entity>()),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}