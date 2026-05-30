using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media;
using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio;

public class GetAudioByStreetcodeIdQueryHandlerTests
{
    private const int StreetcodeId = 1;
    private const string BlobName = "audio-file.mp3";
    private const string Base64Value = "base64encodedcontent";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IMapper _mapper;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IStreetcodeRepository> _streetcodeRepositoryMock;
    private readonly GetAudioByStreetcodeIdQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetAudioByStreetcodeIdQueryHandlerTests"/> class.
    /// </summary>
    public GetAudioByStreetcodeIdQueryHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();

        _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<AudioProfile>())
            .CreateMapper();

        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(w => w.StreetcodeRepository)
            .Returns(_streetcodeRepositoryMock.Object);

        _handler = new GetAudioByStreetcodeIdQueryHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    /// <summary>
    /// Tests that when the streetcode with the specified identifier
    /// is not found, the handler returns a failed result.
    /// </summary>
    [Fact]
    public async Task Handle_WhenStreetcodeNotFound_ReturnsFailResult()
    {
        var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);

        SetupGetFirstOrDefault(null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(string.Format(
                ErrorMessages.CannotFindAudioByStreetcodeId,
                StreetcodeId));
    }

    /// <summary>
    /// Tests that when the streetcode is not found,
    /// the handler logs an error.
    /// </summary>
    [Fact]
    public async Task Handle_WhenStreetcodeNotFound_LogsError()
    {
        var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);

        SetupGetFirstOrDefault(null);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                query,
                string.Format(
                    ErrorMessages.CannotFindAudioByStreetcodeId,
                    StreetcodeId)),
            Times.Once);
    }

    /// <summary>
    /// Tests that when the streetcode exists and contains audio,
    /// the handler returns AudioDTO successfully.
    /// </summary>
    [Fact]
    public async Task Handle_WhenStreetcodeFoundAndAudioExists_ReturnsOkResultWithDTO()
    {
        var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);

        var audio = new AudioEntity
        {
            Id = 1,
            BlobName = BlobName,
        };

        var streetcode = new StreetcodeContent
        {
            Id = StreetcodeId,
            Audio = audio,
        };

        var dto = new AudioDTO
        {
            Id = 1,
            BlobName = BlobName,
            Base64 = Base64Value,
        };

        SetupGetFirstOrDefault(streetcode);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64(BlobName))
            .Returns(Base64Value);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
    }

    /// <summary>
    /// Tests that Base64 is loaded from blob service.
    /// </summary>
    [Fact]
    public async Task Handle_WhenStreetcodeFoundAndAudioExists_SetsBase64FromBlobService()
    {
        var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);

        var audio = new AudioEntity
        {
            Id = 1,
            BlobName = BlobName,
        };

        var streetcode = new StreetcodeContent
        {
            Id = StreetcodeId,
            Audio = audio,
        };

        SetupGetFirstOrDefault(streetcode);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64(BlobName))
            .Returns(Base64Value);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Base64.Should().Be(Base64Value);
    }

    /// <summary>
    /// Tests that when streetcode exists but audio is null,
    /// the handler returns success with null value.
    /// </summary>
    [Fact]
    public async Task Handle_WhenStreetcodeFoundButAudioIsNull_ReturnsNullResultWithNoValue()
    {
        var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);

        var streetcode = new StreetcodeContent
        {
            Id = StreetcodeId,
            Audio = null,
        };

        SetupGetFirstOrDefault(streetcode);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    private void SetupGetFirstOrDefault(StreetcodeContent? returnValue)
    {
        _streetcodeRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>,
                    IIncludableQueryable<StreetcodeContent, object>>?>()))
            .ReturnsAsync(returnValue!);
    }
}