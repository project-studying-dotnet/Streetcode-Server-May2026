using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.MediatR.Media.ArtSlide.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.ArtSlide.Update;

public class UpdateArtSlideHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IStreetcodeArtSlideRepository> _slideRepoMock;
    private readonly Mock<IArtSlideItemRepository> _slideItemRepoMock;
    private readonly IMapper _mapper;
    private readonly UpdateArtSlideHandler _handler;

    public UpdateArtSlideHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArtSlideProfile>();
        });

        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _slideRepoMock = new Mock<IStreetcodeArtSlideRepository>();
        _slideItemRepoMock = new Mock<IArtSlideItemRepository>();

        _repoWrapperMock.Setup(r => r.StreetcodeArtSlideRepository).Returns(_slideRepoMock.Object);
        _repoWrapperMock.Setup(r => r.ArtSlideItemRepository).Returns(_slideItemRepoMock.Object);

        _handler = new UpdateArtSlideHandler(_mapper, _repoWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSlideAndRecreateItems_WhenDataIsValid()
    {
        // Arrange
        var slideId = 1;
        var requestDto = new UpdateArtSlideDto
        {
            Id = slideId,
            Index = 5,
            ArtSlideItems = new List<ArtSlideItemDto> { new() { ArtId = 10, Index = 0 } }
        };
        var command = new UpdateArtSlideCommand(requestDto);
        var existingSlide = new StreetcodeArtSlide { Id = slideId };

        _slideRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeArtSlide, bool>>>(), null))
            .ReturnsAsync(existingSlide);

        _repoWrapperMock.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Проверяем, что обновление состоялось
        existingSlide.Index.Should().Be(5);
        _slideRepoMock.Verify(r => r.Update(It.IsAny<StreetcodeArtSlide>()), Times.Once);

        // Проверяем "пересборку" элементов
        _slideItemRepoMock.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<ArtSlideItem>>()), Times.Once);
        _slideItemRepoMock.Verify(r => r.Create(It.IsAny<ArtSlideItem>()), Times.Once);
        _repoWrapperMock.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSlideNotFound()
    {
        // Arrange
        var requestDto = new UpdateArtSlideDto { Id = 99 };
        _slideRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeArtSlide, bool>>>(), null))
            .ReturnsAsync((StreetcodeArtSlide)null!);

        // Act
        var result = await _handler.Handle(new UpdateArtSlideCommand(requestDto), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains(string.Format(ErrorMessages.SlideNotFound, requestDto.Id)));
    }
}