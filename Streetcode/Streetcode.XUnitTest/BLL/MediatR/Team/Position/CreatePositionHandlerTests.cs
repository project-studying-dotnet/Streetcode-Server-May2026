using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Team.Position;

public class CreatePositionHandlerTests
{
    private const string PositionName = "Developer";
    private const string DatabaseErrorMessage = "Database error";

    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CreatePositionHandler _handler;

    public CreatePositionHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Positions, PositionDTO>();
            cfg.CreateMap<PositionDTO, Positions>();
        }).CreateMapper();

        _mockLogger = new Mock<ILoggerService>();

        _handler = new CreatePositionHandler(
            _mapper,
            _mockRepo.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreatePosition_WhenDataIsValid()
    {
        var positionDto = new PositionDTO
        {
            Position = PositionName,
        };

        var positionEntity = new Positions
        {
            Id = 1,
            Position = PositionName,
        };

        _mockRepo
            .Setup(repo => repo.PositionRepository.CreateAsync(
                It.IsAny<Positions>()))
            .ReturnsAsync(positionEntity);

        _mockRepo
            .Setup(repo => repo.SaveChanges())
            .Returns(1);

        var result = await _handler.Handle(
            new CreatePositionQuery(positionDto),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Position.Should().Be(positionDto.Position);

        _mockRepo.Verify(
            repo => repo.SaveChanges(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaveChangesThrowsException()
    {
        var positionDto = new PositionDTO
        {
            Position = PositionName,
        };

        var positionEntity = new Positions
        {
            Id = 1,
            Position = PositionName,
        };

        _mockRepo
            .Setup(repo => repo.PositionRepository.CreateAsync(
                It.IsAny<Positions>()))
            .ReturnsAsync(positionEntity);

        _mockRepo
            .Setup(repo => repo.SaveChanges())
            .Throws(new Exception(DatabaseErrorMessage));

        var result = await _handler.Handle(
            new CreatePositionQuery(positionDto),
            CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(e => e.Message == DatabaseErrorMessage);

        _mockRepo.Verify(
            repo => repo.SaveChanges(),
            Times.Once);
    }
}