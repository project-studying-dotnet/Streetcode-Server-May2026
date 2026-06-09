using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.SourceLinkCategory.GetAll
{
    public class GetAllCategoriesHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly IMapper _mapper;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllCategoriesHandler _handler;

        public GetAllCategoriesHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SourceLinkCategoryProfile>();
                cfg.AddProfile<ImageProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _handler = new GetAllCategoriesHandler(
                _repositoryWrapperMock.Object,
                _mapper,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenCategoriesAreNull()
        {
            var query = new GetAllCategoriesQuery();

            _repositoryWrapperMock
                .Setup(x => x.SourceCategoryRepository.GetAllAsync(
                    It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<SourceLinkCategoryEntity>,
                        IIncludableQueryable<SourceLinkCategoryEntity, object>>>()))
                .ReturnsAsync((IEnumerable<SourceLinkCategoryEntity>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Be("Categories is null");

            _loggerMock.Verify(
                logger => logger.LogError(query, "Categories is null"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedDtosWithBase64_WhenCategoriesExist()
        {
            var query = new GetAllCategoriesQuery();

            var categories = new List<SourceLinkCategoryEntity>
            {
                new()
                {
                    Id = 1,
                    Title = "Books",
                    Image = new Image
                    {
                        BlobName = "books.png"
                    }
                }
            };

            _repositoryWrapperMock
                .Setup(x => x.SourceCategoryRepository.GetAllAsync(
                    It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<SourceLinkCategoryEntity>,
                        IIncludableQueryable<SourceLinkCategoryEntity, object>>>()))
                .ReturnsAsync(categories);

            _blobServiceMock
                .Setup(blob => blob.FindFileInStorageAsBase64("books.png"))
                .Returns("base64-content");

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var resultDtos = result.Value!.ToList();

            resultDtos.Should().HaveCount(1);
            resultDtos[0].Id.Should().Be(1);
            resultDtos[0].Title.Should().Be("Books");
            resultDtos[0].Image.Should().NotBeNull();
            resultDtos[0].Image!.Base64.Should().Be("base64-content");

            _blobServiceMock.Verify(
                blob => blob.FindFileInStorageAsBase64("books.png"),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}
