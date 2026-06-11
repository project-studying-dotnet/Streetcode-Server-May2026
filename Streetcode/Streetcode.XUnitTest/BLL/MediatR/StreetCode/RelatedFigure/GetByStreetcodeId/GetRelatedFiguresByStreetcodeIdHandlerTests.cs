using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using MockQueryable.Moq;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specifications.Streetcode;
using Xunit;

using RelatedFigureEntity = Streetcode.DAL.Entities.Streetcode.RelatedFigure;
using StreetcodeContentEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;

public class GetRelatedFiguresByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetRelatedFiguresByStreetcodeIdHandler _handler;

    public GetRelatedFiguresByStreetcodeIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(GetRelatedFigureByStreetcodeIdQuery).Assembly);
        }).CreateMapper();
        _mockLogger = new Mock<ILoggerService>();

        _handler = new GetRelatedFiguresByStreetcodeIdHandler(
            _mapper,
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenRelatedFiguresExist_ReturnsOkResult()
    {
        var request = new GetRelatedFigureByStreetcodeIdQuery(1);

        _mockRepositoryWrapper.Setup(
            r => r.StreetcodeRepository.GetAllAsync(It.IsAny<RelatedFiguresSpecification>())
        ).ReturnsAsync(new List<StreetcodeContentEntity>
        {
            new StreetcodeContentEntity { Id = 2 }
        });
        _mockRepositoryWrapper.SetupSequence(
            r => r.RelatedFigureRepository.FindAll(
                It.IsAny<Expression<Func<RelatedFigureEntity, bool>>>()
            )
        ).Returns(new List<RelatedFigureEntity>()
        {
            new()
            {
                TargetId = 1,
                ObserverId = 3,
            }
        }.BuildMock()).Returns(new List<RelatedFigureEntity>()
        {
            new()
            {
                TargetId = 4,
                ObserverId = 1
            }
        }.BuildMock());

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmpty_ReturnsFailResult()
    {
        var request = new GetRelatedFigureByStreetcodeIdQuery(1);

        _mockRepositoryWrapper.Setup(r => r.RelatedFigureRepository
            .FindAll(It.IsAny<Expression<Func<RelatedFigureEntity, bool>>>()))
            .Returns(Enumerable.Empty<RelatedFigureEntity>().AsQueryable());

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        _mockLogger.Verify(l => l.LogError(request, It.IsAny<string>()), Times.Once);
    }
}