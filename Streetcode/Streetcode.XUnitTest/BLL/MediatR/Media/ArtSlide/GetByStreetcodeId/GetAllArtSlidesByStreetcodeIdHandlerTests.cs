using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using MockQueryable.Moq;
using Moq;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.MediatR.Media.ArtSlide.GetAllByStreetcodeId;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.ArtSlide.GetAllByStreetcodeId;

public class GetAllArtSlidesByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IStreetcodeArtSlideRepository> _slideRepoMock;
    private readonly IMapper _mapper;
    private readonly GetAllArtSlidesByStreetcodeIdHandler _handler;

    public GetAllArtSlidesByStreetcodeIdHandlerTests()
    {
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<ArtSlideProfile>(); });
        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _slideRepoMock = new Mock<IStreetcodeArtSlideRepository>();

        _repoWrapperMock.Setup(r => r.StreetcodeArtSlideRepository).Returns(_slideRepoMock.Object);
        _handler = new GetAllArtSlidesByStreetcodeIdHandler(_mapper, _repoWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSlides_WhenStreetcodeIdIsValid()
    {
        // Arrange
        int streetcodeId = 1;
        var command = new GetAllArtSlidesByStreetcodeIdQuery(streetcodeId);

        var slides = new List<StreetcodeArtSlide>
        {
            new() { Id = 1, StreetcodeId = streetcodeId }
        }.AsQueryable().BuildMock();

        // Используем корректный тип Func для include
        _slideRepoMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeArtSlide, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeArtSlide>, IIncludableQueryable<StreetcodeArtSlide, object>>>()))
            .ReturnsAsync(slides.ToList());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        // В Verify используем те же самые типы
        _slideRepoMock.Verify(
            r => r.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeArtSlide, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeArtSlide>, IIncludableQueryable<StreetcodeArtSlide, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSlidesAreNull()
    {
        // Arrange
        _slideRepoMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeArtSlide, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeArtSlide>, IIncludableQueryable<StreetcodeArtSlide, object>>>()))
            .ReturnsAsync((List<StreetcodeArtSlide>)null!);

        // Act
        var result = await _handler.Handle(new GetAllArtSlidesByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Slides not found");
    }
}