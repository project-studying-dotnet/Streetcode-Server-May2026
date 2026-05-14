using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Team;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Team.Position
{
    public class CreatePositionHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<ITeamRepository> _mockTeamRepo;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreatePositionHandler _handler;

        public CreatePositionHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockTeamRepo = new Mock<ITeamRepository>();
            this._mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Positions, PositionDTO>();
                cfg.CreateMap<PositionDTO, Positions>();
            }).CreateMapper();
            _mockLogger = new Mock<ILoggerService>();
            _mockRepo.Setup(x => x.TeamRepository).Returns(_mockTeamRepo.Object);
            _handler = new CreatePositionHandler(_mapper, _mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreatePosition_WhenDataIsValid()
        {
            var positionDTO = new PositionDTO { Position = "Developer" };
            var positionEntity = new Positions { Id = 1, Position = "Developer" };

            _mockRepo.Setup(repo => repo.PositionRepository.CreateAsync(It.IsAny<Positions>()))
                .ReturnsAsync(positionEntity);

            _mockRepo.Setup(repo => repo.SaveChanges()).Returns(1); 

            var result = await _handler.Handle(new CreatePositionQuery(positionDTO), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Position.Should().Be(positionDTO.Position);

            _mockRepo.Verify(repo => repo.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSaveChangesThrowsException()
        {
 
            var positionDTO = new PositionDTO { Position = "Developer" };
            var positionEntity = new Positions { Id = 1, Position = "Developer" };
            _mockRepo.Setup(repo => repo.PositionRepository.CreateAsync(It.IsAny<Positions>()))
                .ReturnsAsync(positionEntity);
           
            _mockRepo.Setup(repo => repo.SaveChanges()).Throws(new Exception("Database error"));
            
            var result = await _handler.Handle(new CreatePositionQuery(positionDTO), CancellationToken.None);
        
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == "Database error");
           
            _mockRepo.Verify(repo => repo.SaveChanges(), Times.Once);
        }
    }
}