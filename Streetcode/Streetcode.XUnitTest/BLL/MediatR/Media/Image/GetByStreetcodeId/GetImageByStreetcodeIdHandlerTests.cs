using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Image.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using ImageEntity = Streetcode.DAL.Entities.Media.Images.Image;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Image.GetByStreetcodeId;

public class GetImageByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetImageByStreetcodeIdHandler _handler;

    public GetImageByStreetcodeIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockBlobService = new Mock<IBlobService>();
        _mockLogger = new Mock<ILoggerService>();

        _handler = new GetImageByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenImagesExist_ReturnsOkResultWithBase64()
    {
        var streetcodeId = 1;
        var request = new GetImageByStreetcodeIdQuery(streetcodeId);
        var images = new List<ImageEntity> { new() { BlobName = "test-blob" } };
        var imageDtos = new List<ImageDTO> { new() { BlobName = "test-blob" } };

        // Перекриваємо всі можливі виклики GetAllAsync
        _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetAllAsync(
            It.IsAny<Expression<Func<ImageEntity, bool>>>(),
            It.IsAny<Func<IQueryable<ImageEntity>, IIncludableQueryable<ImageEntity, object>>>()))
            .ReturnsAsync(images);

        _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetAllAsync(
            It.IsAny<Expression<Func<ImageEntity, bool>>>(),
            null))
            .ReturnsAsync(images);

        _mockMapper.Setup(m => m.Map<IEnumerable<ImageDTO>>(It.IsAny<IEnumerable<ImageEntity>>()))
            .Returns(imageDtos);

        _mockBlobService.Setup(b => b.FindFileInStorageAsBase64(It.IsAny<string>()))
            .Returns("base64-string");

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.First().Base64.Should().Be("base64-string");
    }

    [Fact]
    public async Task Handle_WhenImagesDoNotExist_ReturnsFailResultAndLogsError()
    {
        var request = new GetImageByStreetcodeIdQuery(99);

        _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetAllAsync(
            It.IsAny<Expression<Func<ImageEntity, bool>>>(),
            It.IsAny<Func<IQueryable<ImageEntity>, IIncludableQueryable<ImageEntity, object>>>()))
            .ReturnsAsync(new List<ImageEntity>());

        _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetAllAsync(
            It.IsAny<Expression<Func<ImageEntity, bool>>>(),
            null))
            .ReturnsAsync(new List<ImageEntity>());

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Cannot find an image"));
        _mockLogger.Verify(l => l.LogError(request, It.IsAny<string>()), Times.Once);
    }
}