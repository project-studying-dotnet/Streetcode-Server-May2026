using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.GetAll;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Team.TeamMemberLinks
{
    public class GetAllTeamLinkHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllTeamLinkHandler _handler;
        public GetAllTeamLinkHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            this._mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TeamMemberLink, TeamMemberLinkDTO>();
            }).CreateMapper();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new GetAllTeamLinkHandler(_mockRepo.Object, _mapper, _mockLogger.Object);
        }
        [Fact]
        public async Task Handle_ReturnsOkResult_WhenTeamLinksExist()
        {
            var teamLinkEntities = new List<TeamMemberLink>
            {
                new TeamMemberLink { Id = 1, TargetUrl = "http://example.com", TeamMemberId = 1 },
                new TeamMemberLink { Id = 2, TargetUrl = "http://example.org", TeamMemberId = 2 }
            };
            var teamLinkDTOs = teamLinkEntities.Select(tl => this._mapper.Map<TeamMemberLinkDTO>(tl)).ToList();
            _mockRepo.Setup(repo => repo.TeamLinkRepository.GetAllAsync()).ReturnsAsync(teamLinkEntities);
            var result = await _handler.Handle(new GetAllTeamLinkQuery(), CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(teamLinkDTOs);
        }
        [Fact]
        public async Task Handle_ReturnsFailResult_WhenNoTeamLinksExist()
        {
            _mockRepo.Setup(repo => repo.TeamLinkRepository.GetAllAsync()).ReturnsAsync((IEnumerable<TeamMemberLink>)null);
            var result = await _handler.Handle(new GetAllTeamLinkQuery(), CancellationToken.None);
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == "Cannot find any team links");
        }
    }
}
