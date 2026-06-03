using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using FluentAssertions;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId
{
    public class GetCoordinatesByStreetcodeIdHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetCoordinatesByStreetcodeIdHandler _handler;

        public GetCoordinatesByStreetcodeIdHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
               cfg.AddMaps(typeof(GetCoordinatesByStreetcodeIdHandler).Assembly);
            }).CreateMapper();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetCoordinatesByStreetcodeIdHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_StreetcodeDoesNotExist_ReturnsFailResult()
        {
            var query = new GetCoordinatesByStreetcodeIdQuery(1);

            _repositoryWrapperMock.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
                .ReturnsAsync((StreetcodeContent)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task Handle_CoordinatesFound_ReturnsOkResult()
        {
            var query = new GetCoordinatesByStreetcodeIdQuery(1);
            var streetcode = new StreetcodeContent { Id = 1 };
            var coordinates = new List<StreetcodeCoordinate> { new StreetcodeCoordinate() };
            var dtos = _mapper.Map<IEnumerable<StreetcodeCoordinateDTO>>(coordinates);

            _repositoryWrapperMock.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
                .ReturnsAsync(streetcode);

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(), null))
                .ReturnsAsync(coordinates);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dtos);
        }
    }
}