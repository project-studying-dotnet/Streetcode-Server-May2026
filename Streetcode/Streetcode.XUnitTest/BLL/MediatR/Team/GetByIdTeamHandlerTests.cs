using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetById;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Team;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Team;

public class GetByIdTeamHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<ITeamRepository> _mockTeamRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetByIdTeamHandler _handler;

    public GetByIdTeamHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockTeamRepo = new Mock<ITeamRepository>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TeamMember, TeamMemberDTO>();
        }).CreateMapper();

        _mockLogger = new Mock<ILoggerService>();

        _mockRepo
            .Setup(x => x.TeamRepository)
            .Returns(_mockTeamRepo.Object);

        _handler = new GetByIdTeamHandler(
            _mockRepo.Object,
            _mapper,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOkResult_WhenTeamExists()
    {
        var teamEntity = new TeamMember
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
        };

        var teamDto = _mapper.Map<TeamMemberDTO>(teamEntity);

        _mockTeamRepo
            .Setup(repo => repo.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>,
                    IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(teamEntity);

        var result = await _handler.Handle(
            new GetByIdTeamQuery(1),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(teamDto);
    }

    [Fact]
    public async Task Handle_ReturnsFailResult_WhenTeamDoesNotExist()
    {
        _mockTeamRepo
            .Setup(repo => repo.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>,
                    IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((TeamMember)null!);

        var result = await _handler.Handle(
            new GetByIdTeamQuery(1),
            CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(e => e.Message.Contains("Cannot find"));
    }
}
