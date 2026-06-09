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
using Streetcode.DAL.Repositories.Interfaces.Team;
using Xunit;

namespace Streetcode.XUnitTest.MediatR.Team.GetAll;

public class GetAllTeamHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<ITeamRepository> _mockTeamRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllTeamHandler _handler;

    public GetAllTeamHandlerTests()
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

        _handler = new GetAllTeamHandler(
            _mockRepo.Object,
            _mapper,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOkResult_WhenTeamExists()
    {
        var teamEntities = new List<TeamMember>
        {
            new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
            },
            new()
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
            },
        };

        var teamDtos = teamEntities
            .Select(_mapper.Map<TeamMemberDTO>)
            .ToList();

        _mockTeamRepo
            .Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>,
                    IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(teamEntities);

        var query = new GetAllTeamQuery();

        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(teamDtos);
    }

    [Fact]
    public async Task Handle_ReturnsFailResult_WhenNoTeamExists()
    {
        _mockTeamRepo
            .Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>,
                    IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((IEnumerable<TeamMember>)null!);

        var query = new GetAllTeamQuery();

        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(e => e.Message == "Cannot find any team");
    }
}
