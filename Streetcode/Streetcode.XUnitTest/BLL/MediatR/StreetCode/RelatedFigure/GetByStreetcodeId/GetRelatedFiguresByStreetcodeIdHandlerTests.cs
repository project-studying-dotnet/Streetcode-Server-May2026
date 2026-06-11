using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using RelatedFigureEntity = Streetcode.DAL.Entities.Streetcode.RelatedFigure;
using StreetcodeContentEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;

public class GetRelatedFiguresByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetRelatedFiguresByStreetcodeIdHandler _handler;

    public GetRelatedFiguresByStreetcodeIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetRelatedFiguresByStreetcodeIdHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenRelatedFiguresExist_ReturnsOkResult()
    {
        var request = new GetRelatedFigureByStreetcodeIdQuery(1);

        _mockRepositoryWrapper.Setup(static r => r.RelatedFigureRepository
            .FindAll(It.IsAny<Expression<Func<RelatedFigureEntity, bool>>>()))
            .Returns(new List<RelatedFigureEntity>
            {
                new RelatedFigureEntity { TargetId = 2, ObserverId = 1 }
            }.AsQueryable());

        _mockRepositoryWrapper.Setup(static r => r.StreetcodeRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContentEntity>, IIncludableQueryable<StreetcodeContentEntity, object>>>()))
            .ReturnsAsync(new List<StreetcodeContentEntity>
            {
                new StreetcodeContentEntity { Id = 2 }
            });

        _mockMapper.Setup(static m => m.Map<IEnumerable<RelatedFigureDTO>>(It.IsAny<IEnumerable<StreetcodeContentEntity>>()))
            .Returns(new List<RelatedFigureDTO> { new RelatedFigureDTO { Id = 2 } });

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ReturnsFailResult()
    {
        var request = new GetRelatedFigureByStreetcodeIdQuery(1);

        _mockRepositoryWrapper.Setup(static r => r.RelatedFigureRepository
            .FindAll(It.IsAny<Expression<Func<RelatedFigureEntity, bool>>>()))
            .Returns(new List<RelatedFigureEntity>
            {
                new RelatedFigureEntity { TargetId = 2, ObserverId = 1 }
            }.AsQueryable());

        _mockRepositoryWrapper.Setup(static r => r.StreetcodeRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContentEntity>, IIncludableQueryable<StreetcodeContentEntity, object>>>()))
            .ReturnsAsync((IEnumerable<StreetcodeContentEntity>)null!);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        _mockLogger.Verify(l => l.LogError(request, It.IsAny<string>()), Times.Once);
    }
}