using System.Linq.Expressions;

using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;

using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;
using StreetcodeContentEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId
{
    public class GetCategoryContentByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetCategoryContentByStreetcodeIdHandler _handler;

        public GetCategoryContentByStreetcodeIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SourceLinkCategoryProfile>();
                cfg.AddProfile<StreetcodeCategoryContentProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _handler = new GetCategoryContentByStreetcodeIdHandler(
                _repositoryWrapperMock.Object,
                _mapper,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnStreetcodeCategoryContentDTO_WhenStreetcodeAndContentExist()
        {
            var query = new GetCategoryContentByStreetcodeIdQuery(1, 2);

            var streetcodeEntity = new StreetcodeContentEntity
            {
                Id = query.streetcodeId
            };

            var streetcodeCategoryContentEntity = new StreetcodeCategoryContentEntity
            {
                StreetcodeId = query.streetcodeId,
                SourceLinkCategoryId = query.categoryId,
                Text = "Test content"
            };

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContentEntity>,
                        IIncludableQueryable<StreetcodeContentEntity, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(streetcodeEntity);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.StreetcodeCategoryContentRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeCategoryContentEntity>,
                        IIncludableQueryable<StreetcodeCategoryContentEntity, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(streetcodeCategoryContentEntity);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var resultDto = result.Value!;

            resultDto.StreetcodeId.Should().Be(query.streetcodeId);
            resultDto.SourceLinkCategoryId.Should().Be(query.categoryId);
            resultDto.Text.Should().Be("Test content");

            _loggerMock.Verify(
                logger => logger.LogError(
                    It.IsAny<GetCategoryContentByStreetcodeIdQuery>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenStreetcodeDoesNotExist()
        {
            var query = new GetCategoryContentByStreetcodeIdQuery(1, 2);
            var errorMsg = $"No such streetcode with id = {query.streetcodeId}";

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContentEntity>,
                        IIncludableQueryable<StreetcodeContentEntity, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((StreetcodeContentEntity?)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Be(errorMsg);

            _loggerMock.Verify(
                logger => logger.LogError(query, errorMsg),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.StreetcodeCategoryContentRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeCategoryContentEntity>,
                        IIncludableQueryable<StreetcodeCategoryContentEntity, object>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenStreetcodeCategoryContentDoesNotExist()
        {
            var query = new GetCategoryContentByStreetcodeIdQuery(1, 2);
            var errorMsg = "The streetcode content is null";

            var streetcodeEntity = new StreetcodeContentEntity
            {
                Id = query.streetcodeId
            };

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContentEntity>,
                        IIncludableQueryable<StreetcodeContentEntity, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(streetcodeEntity);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.StreetcodeCategoryContentRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeCategoryContentEntity>,
                        IIncludableQueryable<StreetcodeCategoryContentEntity, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((StreetcodeCategoryContentEntity?)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Be(errorMsg);

            _loggerMock.Verify(
                logger => logger.LogError(query, errorMsg),
                Times.Once);
        }
    }
}
