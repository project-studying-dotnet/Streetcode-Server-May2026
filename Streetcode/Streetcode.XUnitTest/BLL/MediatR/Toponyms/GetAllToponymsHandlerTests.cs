using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Toponyms.GetAll;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Toponyms;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms;

public sealed class GetAllToponymsHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IToponymRepository> _toponymRepositoryMock;
    private readonly GetAllToponymsHandler _handler;

    public GetAllToponymsHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Toponym, ToponymDTO>();
        }).CreateMapper();

        _loggerMock = new Mock<ILoggerService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _toponymRepositoryMock = new Mock<IToponymRepository>();

        _repositoryWrapperMock
            .Setup(r => r.ToponymRepository)
            .Returns(_toponymRepositoryMock.Object);

        _handler = new GetAllToponymsHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllToponyms_WhenNoTitleProvided()
    {
        IQueryable<Toponym> toponyms = new List<Toponym>
        {
            new()
            {
                Id = 1,
                StreetName = "Шевченка",
            },
            new()
            {
                Id = 2,
                StreetName = "Бандери",
            },
        }.AsQueryable();

        List<ToponymDTO> expectedToponyms = new()
        {
            _mapper.Map<ToponymDTO>(toponyms.ElementAt(0)),
            _mapper.Map<ToponymDTO>(toponyms.ElementAt(1)),
        };

        GetAllToponymsQuery query = new(new GetAllToponymsRequestDTO
        {
            Title = null,
        });

        _toponymRepositoryMock
            .Setup(r => r.FindAll(null))
            .Returns(toponyms);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Toponyms.Should().BeEquivalentTo(expectedToponyms);

        _toponymRepositoryMock.Verify(
            r => r.FindAll(null),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilterToponyms_WhenTitleProvided_CaseInsensitive()
    {
        IQueryable<Toponym> toponyms = new List<Toponym>
        {
            new()
            {
                Id = 1,
                StreetName = "Шевченка",
            },
            new()
            {
                Id = 2,
                StreetName = "Бандери",
            },
        }.AsQueryable();

        List<ToponymDTO> expectedToponyms = new()
        {
            _mapper.Map<ToponymDTO>(toponyms.ElementAt(0)),
        };

        GetAllToponymsQuery query = new(new GetAllToponymsRequestDTO
        {
            Title = "шЕв",
        });

        _toponymRepositoryMock
            .Setup(r => r.FindAll(null))
            .Returns(toponyms);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Toponyms.Should().BeEquivalentTo(expectedToponyms);

        _toponymRepositoryMock.Verify(
            r => r.FindAll(null),
            Times.Once);
    }
}