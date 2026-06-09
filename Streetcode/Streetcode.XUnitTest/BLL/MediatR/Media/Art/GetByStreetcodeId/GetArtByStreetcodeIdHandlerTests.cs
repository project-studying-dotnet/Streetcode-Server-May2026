using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specifications.Base;
using Xunit;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.GetByStreetcodeId;

/// <summary>
/// Checking class GetArtByStreetcodeIdHandler.
/// </summary>
public class GetArtByStreetcodeIdHandlerTests
{
    private readonly GetArtsByStreetcodeIdHandler _handler;
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly Mock<IArtRepository> _mockArtRepository;
    private readonly IMapper _mapper;

    public GetArtByStreetcodeIdHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockBlobService = new Mock<IBlobService>();
        _mockLoggerService = new Mock<ILoggerService>();
        _mockArtRepository = new Mock<IArtRepository>();

        _mockRepository
            .Setup(r => r.ArtRepository)
            .Returns(_mockArtRepository.Object);

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ArtEntity, ArtDTO>();
        }).CreateMapper();

        _handler = new GetArtsByStreetcodeIdHandler(
            _mockRepository.Object,
            _mapper,
            _mockBlobService.Object,
            _mockLoggerService.Object);
    }

    [Fact]
    public async Task Handle_ValidStreetcodeId_ReturnCorrectArts()
    {
        // Arrange
        var query = new GetArtsByStreetcodeIdQuery(1);
        const string expectedTitle = "Title art 1";

        var arts = new List<ArtEntity>
        {
            new()
            {
                Id = 1,
                Description = "Description art 1",
                ImageId = 1,
                Title = expectedTitle,
                StreetcodeArts = new List<StreetcodeArt>
                {
                    new()
                    {
                        ArtId = 1,
                        StreetcodeId = 1,
                    },
                },
            },
        };

        _mockArtRepository
            .Setup(r => r.GetAllAsync(It.IsAny<ISpecification<ArtEntity>>()))
            .ReturnsAsync(arts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        var art = Assert.Single(result.Value);
        art.Title.Should().Be(expectedTitle);
        art.Should().BeOfType<ArtDTO>();

        Assert.All(result.Value, item =>
        {
            Assert.IsType<ArtDTO>(item);
        });

        _mockArtRepository.Verify(
            r => r.GetAllAsync(It.IsAny<ISpecification<ArtEntity>>()),
            Times.Once);
    }

    /// <summary>
    /// Method returns error message, if streetcode not exist arts.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_ValidStretcodeIdWithEmptyArts_ReturnErrorMessage()
    {
        // Arrange
        var query = new GetArtsByStreetcodeIdQuery(1);
        var expectedErrorMessage = string.Format(
            ErrorMessages.CannotFindArtByStreetcodeId,
            query.StreetcodeId);

        _mockArtRepository
            .Setup(r => r.GetAllAsync(It.IsAny<ISpecification<ArtEntity>>()))
            .ReturnsAsync((IEnumerable<ArtEntity>)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be(expectedErrorMessage);

        _mockArtRepository.Verify(
            r => r.GetAllAsync(It.IsAny<ISpecification<ArtEntity>>()),
            Times.Once);
    }
}