using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Toponyms.GetById;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Toponyms;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms;

public sealed class GetToponymByIdHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IToponymRepository> _toponymRepositoryMock;
    private readonly GetToponymByIdHandler _handler;

    public GetToponymByIdHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Toponym, ToponymDTO>();
        }).CreateMapper();

        _loggerMock = new Mock<ILoggerService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _toponymRepositoryMock = new Mock<IToponymRepository>();

        _repositoryWrapperMock
            .Setup(r => r.ToponymRepository)
            .Returns(_toponymRepositoryMock.Object);

        _handler = new GetToponymByIdHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnToponym_WhenFound()
    {
        var toponym = new Toponym
        {
            Id = 2,
            StreetName = "Бандери",
        };

        var expectedToponym = _mapper.Map<ToponymDTO>(toponym);
        var query = new GetToponymByIdQuery(2);

        _toponymRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Toponym, bool>>>(),
                null))
            .ReturnsAsync(toponym);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedToponym);

        _toponymRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Toponym, bool>>>(),
                null),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenNotFound()
    {
        var query = new GetToponymByIdQuery(1);

        _toponymRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Toponym, bool>>>(),
                null))
            .ReturnsAsync((Toponym)null!);

        _loggerMock
            .Setup(l => l.LogError(query, It.IsAny<string>()));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _toponymRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Toponym, bool>>>(),
                null),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(query, It.IsAny<string>()),
            Times.Once);
    }
}
