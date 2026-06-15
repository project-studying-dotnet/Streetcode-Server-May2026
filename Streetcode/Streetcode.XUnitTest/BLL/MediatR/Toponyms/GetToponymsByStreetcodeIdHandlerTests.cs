using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Toponyms;
using Streetcode.DAL.Specifications.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms;

/// <summary>
/// Initializes a new instance of the <see cref="GetToponymsByStreetcodeIdHandlerTests"/> class.
/// </summary>
public sealed class GetToponymsByStreetcodeIdHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IToponymRepository> _toponymRepositoryMock;
    private readonly GetToponymsByStreetcodeIdHandler _handler;

    public GetToponymsByStreetcodeIdHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Toponym, ToponymDTO>();
            cfg.CreateMap<ToponymCoordinate, ToponymCoordinateDTO>();
            cfg.CreateMap<StreetcodeContent, StreetcodeDTO>();
            cfg.CreateMap<Tag, StreetcodeTagDTO>();
        }).CreateMapper();

        _loggerMock = new Mock<ILoggerService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _toponymRepositoryMock = new Mock<IToponymRepository>();

        _repositoryWrapperMock
            .Setup(r => r.ToponymRepository)
            .Returns(_toponymRepositoryMock.Object);

        _handler = new GetToponymsByStreetcodeIdHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnToponyms_WhenFound()
    {
        // Arrange
        IQueryable<Toponym> toponyms = new List<Toponym>()
        {
            new()
            {
                Id = 1,
                StreetName = "Шевченка",
                Oblast = "Київська",
                Streetcodes = new List<StreetcodeContent>()
                {
                    new()
                    {
                        Id = 1,
                    },
                    new()
                    {
                        Id = 2,
                    },
                },
            },
        }.BuildMock();

        List<ToponymDTO> expectedToponyms = new()
        {
            _mapper.Map<ToponymDTO>(toponyms.First()),
        };

        GetToponymsByStreetcodeIdQuery query = new(1);

        _toponymRepositoryMock.Setup(
            r => r.FindAll(It.IsAny<ISpecification<Toponym>>())
        ).Returns(toponyms);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedToponyms);
        _toponymRepositoryMock.Verify(
            r => r.FindAll(It.IsAny<ISpecification<Toponym>>()),
            Times.Once
        );
    }

    /// <summary>
    /// Should return toponyms with unique street name when found by streetcode id.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    [Fact]
    public async Task Handle_ShouldReturnUniqueToponyms_WhenFound()
    {
        // Arrange
        var query = new GetToponymsByStreetcodeIdQuery(1);

        var toponyms = new List<Toponym>
        {
            CreateToponym(1, "Шевченка", "Київська"),
            CreateToponym(2, "Шевченка", "Львівська"),
        }.BuildMock();

        var expectedToponyms = new List<ToponymDTO>
        {
            _mapper.Map<ToponymDTO>(toponyms.First()),
        };

        _toponymRepositoryMock
            .Setup(r => r.FindAll(
                It.IsAny<ISpecification<Toponym>>()))
            .Returns(toponyms);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedToponyms);

        _toponymRepositoryMock.Verify(
            r => r.FindAll(
                It.IsAny<ISpecification<Toponym>>()),
            Times.Once);
    }

    /// <summary>
    /// Should return error and log it when toponyms not found by streetcode id.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    [Fact]
    public async Task Handle_ShouldReturnError_WhenNotFound()
    {
        // Arrange
        GetToponymsByStreetcodeIdQuery query = new(3);

        _toponymRepositoryMock.Setup(
            r => r.FindAll(It.IsAny<ISpecification<Toponym>>())
        ).Returns(Enumerable.Empty<Toponym>().BuildMock());

        _loggerMock.Setup(
            l => l.LogError(query, It.IsAny<string>())
        );

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _toponymRepositoryMock.Verify(
            r => r.FindAll(It.IsAny<ISpecification<Toponym>>()),
            Times.Once
        );

        _loggerMock.Verify(
            l => l.LogError(query, It.IsAny<string>()),
            Times.Once
        );
    }

    private static Toponym CreateToponym(
        int id,
        string streetName,
        string oblast) =>
        new()
        {
            Id = id,
            StreetName = streetName,
            Oblast = oblast,
            Streetcodes = new List<StreetcodeContent>
            {
                new()
                {
                    Id = 1,
                },
                new()
                {
                    Id = 2,
                },
            }
        };
}