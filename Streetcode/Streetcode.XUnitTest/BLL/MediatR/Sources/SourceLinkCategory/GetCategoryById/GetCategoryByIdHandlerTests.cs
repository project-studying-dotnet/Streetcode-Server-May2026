using System.Linq.Expressions;

using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.DAL.Entities.Media.Images;

using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.SourceLinkCategory.GetCaregoryById
{
    public class GetCategoryByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly GetCategoryByIdHandler _handler;

        public GetCategoryByIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetCategoryByIdHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenCategoryDoesNotExist()
        {
            var query = new GetCategoryByIdQuery(1);
            var errorMsg = $"Cannot find any srcCategory by the corresponding id: {query.Id}";
            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SourceCategoryRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<SourceLinkCategoryEntity>,
                        IIncludableQueryable<SourceLinkCategoryEntity, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((SourceLinkCategoryEntity?)null);
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Be(errorMsg);
            _loggerMock.Verify(logger => logger.LogError(query, errorMsg), Times.Once);
            _mapperMock.Verify(
                mapper => mapper.Map<SourceLinkCategoryDTO>(It.IsAny<SourceLinkCategoryEntity>()),
                Times.Never);
            _blobServiceMock.Verify(
                blob => blob.FindFileInStorageAsBase64(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSourceLinkCategoryDTO_WhenCategoryExists()
        {
            var query = new GetCategoryByIdQuery(1);
            var blobName = "test-blob";
            var base64 = "test-base64";

            var image = new Image
            {
                Id = 10,
                BlobName = blobName
            };
            var categoryEntity = new SourceLinkCategoryEntity
            {
                Id = query.Id,
                Image = image
            };
            var categoryDto = new SourceLinkCategoryDTO
            {
                Id = query.Id,
                Image = new ImageDTO
                {
                    Id = 10,
                    BlobName = blobName
                }
            };
            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SourceCategoryRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<SourceLinkCategoryEntity>,
                        IIncludableQueryable<SourceLinkCategoryEntity, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoryEntity);
            _mapperMock
                .Setup(mapper => mapper.Map<SourceLinkCategoryDTO>(categoryEntity))
                .Returns(categoryDto);
            _blobServiceMock
                .Setup(blob => blob.FindFileInStorageAsBase64(blobName))
                .Returns(base64);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var resultDto = result.Value!;

            resultDto.Id.Should().Be(query.Id);
            resultDto.Image.Should().NotBeNull();
            resultDto.Image!.Id.Should().Be(10);
            resultDto.Image.Base64.Should().Be(base64);
            resultDto.Image.BlobName.Should().Be(blobName);

            _mapperMock.Verify(
                mapper => mapper.Map<SourceLinkCategoryDTO>(categoryEntity),
                Times.Once);
            _blobServiceMock.Verify(
                blob => blob.FindFileInStorageAsBase64(blobName),
                Times.Once);
            _loggerMock.Verify(
                 logger => logger.LogError(
                     It.IsAny<GetCategoryByIdQuery>(),
                     It.IsAny<string>()),
                 Times.Never);
        }
    }
}