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
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.Resources;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTerm.GetAllByTermId
{
    public class GetRelatedTermByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IRelatedTermRepository> _relatedTermRepositoryMock;
        private readonly GetAllRelatedTermsByTermIdHandler _handler;

        public GetRelatedTermByIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _relatedTermRepositoryMock = new Mock<IRelatedTermRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.RelatedTermRepository)
                .Returns(_relatedTermRepositoryMock.Object);
            _handler = new GetAllRelatedTermsByTermIdHandler(
              _mapperMock.Object,
              _repositoryWrapperMock.Object,
              _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRelatedTermsIsNull()
        {
            var query = new GetAllRelatedTermsByTermIdQuery(1);

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>,
                    IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync((IEnumerable<Entity>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.CannotGetWordsByTermId);

            _loggerMock.Verify(
                logger => logger.LogError(query, ErrorMessages.CannotGetWordsByTermId),
                Times.Once);

            _mapperMock.Verify(
                mapper => mapper.Map<IEnumerable<RelatedTermDTO>>(It.IsAny<IEnumerable<Entity>>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMappingToDtoFails()
        {
            var query = new GetAllRelatedTermsByTermIdQuery(1);

            var relatedTerms = new List<Entity>
            {
                new Entity { Id = 1, TermId = 1, Word = "test" }
            };

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync(relatedTerms);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<RelatedTermDTO>>(relatedTerms))
                .Returns((IEnumerable<RelatedTermDTO>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.CannotCreateRelatedWordDtos);

            _loggerMock.Verify(
                logger => logger.LogError(query, ErrorMessages.CannotCreateRelatedWordDtos),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenRelatedTermsExist()
        {
            var query = new GetAllRelatedTermsByTermIdQuery(1);

            var relatedTerms = new List<Entity>
            {
                new Entity
                {
                    Id = 1,
                    TermId = 1,
                    Word = "test"
                }
            };

            var relatedTermsDto = new List<RelatedTermDTO>
            {
                new RelatedTermDTO
                {
                    Id = 1,
                    TermId = 1,
                    Word = "test"
                }
            };

            _relatedTermRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>?>()))
                .ReturnsAsync(relatedTerms);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<RelatedTermDTO>>(relatedTerms))
                .Returns(relatedTermsDto);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(relatedTermsDto);

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}