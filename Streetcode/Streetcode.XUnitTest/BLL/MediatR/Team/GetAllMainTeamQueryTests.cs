using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetAll;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatR.Team.GetAll;

public class GetAllMainTeamHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllMainTeamHandler _handler;

    public GetAllMainTeamHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TeamMember, TeamMemberDTO>();
        }).CreateMapper();

        _mockLogger = new Mock<ILoggerService>();

        _handler = new GetAllMainTeamHandler(
            _mockRepo.Object,
            _mapper,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOkResult_WhenMainTeamExists()
    {
        var mainTeamEntities = new List<TeamMember>
        {
            new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                IsMain = true,
            },
            new()
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                IsMain = true,
            },
        };

        var mainTeamDtos = mainTeamEntities
            .Select(_mapper.Map<TeamMemberDTO>)
            .ToList();

        _mockRepo
            .Setup(repo => repo.TeamRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>,
                    IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(mainTeamEntities);

        var result = await _handler.Handle(
            new GetAllMainTeamQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        result.Value.Should()
            .BeEquivalentTo(mainTeamDtos);
    }

    [Fact]
    public async Task Handle_ReturnsFailResult_WhenNoMainTeamExists()
    {
        _mockRepo
            .Setup(repo => repo.TeamRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>,
                    IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((IEnumerable<TeamMember>)null!);

        var result = await _handler.Handle(
            new GetAllMainTeamQuery(),
            CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(e => e.Message == "Cannot find any team");
    }
}
