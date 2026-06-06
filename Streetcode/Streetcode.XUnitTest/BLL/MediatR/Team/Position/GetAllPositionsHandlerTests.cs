using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.Position.GetAll;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Team;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Team.Position
{
    public class GetAllPositionsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<IPositionRepository> _mockPositionRepo;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllPositionsHandler _handler;

        public GetAllPositionsHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockPositionRepo = new Mock<IPositionRepository>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Positions, PositionDTO>();
            }).CreateMapper();

            _mockLogger = new Mock<ILoggerService>();

            _mockRepo.Setup(x => x.PositionRepository).Returns(_mockPositionRepo.Object);

            _handler = new GetAllPositionsHandler(_mockRepo.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPositions_WhenPositionsExist()
        {
            var positions = new List<Positions>
            {
                new Positions { Id = 1, Position = "Position 1" },
                new Positions { Id = 2, Position = "Position 2" }
            };

            _mockPositionRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<Positions, bool>>>(),
                It.IsAny<Func<IQueryable<Positions>,
                IIncludableQueryable<Positions, object>>>())).ReturnsAsync(positions);

            var result = await _handler.Handle(new GetAllPositionsQuery(), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(_mapper.Map<IEnumerable<PositionDTO>>(positions));
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyCollection_WhenNoPositionsExist()
        {
            _mockPositionRepo
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Positions, bool>>>(),
                    It.IsAny<Func<IQueryable<Positions>, IIncludableQueryable<Positions, object>>>()))
                .ReturnsAsync(Array.Empty<Positions>());

            var result = await _handler.Handle(
                new GetAllPositionsQuery(),
                CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}
