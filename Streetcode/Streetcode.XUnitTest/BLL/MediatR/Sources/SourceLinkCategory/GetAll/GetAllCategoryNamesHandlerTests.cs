using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;

using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.SourceLinkCategory.GetAll
{
    public class GetAllCategoryNamesHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllCategoryNamesHandler _handler;

        public GetAllCategoryNamesHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetAllCategoryNamesHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenCategoriesAreNull()
        {
            var query = new GetAllCategoryNamesQuery();

            _repositoryWrapperMock
                .Setup(x => x.SourceCategoryRepository.GetAllAsync(null, null))
                .ReturnsAsync((IEnumerable<SourceLinkCategoryEntity>?)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Be("Categories is null");

            _loggerMock.Verify(
                logger => logger.LogError(query, "Categories is null"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedCategoryNames_WhenCategoriesExist()
        {
            var query = new GetAllCategoryNamesQuery();

            var categories = new List<SourceLinkCategoryEntity>
            {
                new()
                {
                    Id = 1,
                    Title = "Books"
                }
            };

            var dtos = new List<CategoryWithNameDTO>
            {
                new()
                {
                    Id = 1,
                    Title = "Books"
                }
            };

            _repositoryWrapperMock
                .Setup(x => x.SourceCategoryRepository.GetAllAsync(null, null))
                .ReturnsAsync(categories);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<CategoryWithNameDTO>>(categories))
                .Returns(dtos);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var resultDtos = result.Value!.ToList();

            resultDtos.Should().HaveCount(1);
            resultDtos[0].Id.Should().Be(1);
            resultDtos[0].Title.Should().Be("Books");

            _mapperMock.Verify(
                mapper => mapper.Map<IEnumerable<CategoryWithNameDTO>>(categories),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}