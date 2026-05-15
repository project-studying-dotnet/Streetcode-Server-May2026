using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Team.TeamMemberLinks
{
    public class CreateTeamLinkHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateTeamLinkHandler _handler;

        public CreateTeamLinkHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            this._mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TeamMemberLink, TeamMemberLinkDTO>();
                cfg.CreateMap<TeamMemberLinkDTO, TeamMemberLink>();
            }).CreateMapper();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new CreateTeamLinkHandler(_mapper, _mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenTeamLinkIsCreated()
        {
            var teamLinkDTO = new TeamMemberLinkDTO { Id = 1, TargetUrl = "http://example.com", TeamMemberId = 1 };
            var teamLinkEntity = this._mapper.Map<TeamMemberLink>(teamLinkDTO);
            _mockRepo.Setup(repo => repo.TeamLinkRepository.Create(It.IsAny<TeamMemberLink>())).Returns(teamLinkEntity);
            _mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            var result = await _handler.Handle(new CreateTeamLinkQuery(teamLinkDTO), CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(teamLinkDTO);
        }

        [Fact]
        public async Task Handle_ReturnsFailResult_WhenTeamLinkCreationFails()
        {
            var teamLinkDTO = new TeamMemberLinkDTO { Id = 1, TargetUrl = "http://example.com", TeamMemberId = 1 };
            _mockRepo.Setup(repo => repo.TeamLinkRepository.Create(It.IsAny<TeamMemberLink>())).Returns((TeamMemberLink)null);
            var result = await _handler.Handle(new CreateTeamLinkQuery(teamLinkDTO), CancellationToken.None);
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == "Cannot create team link");
        }
    }
}
