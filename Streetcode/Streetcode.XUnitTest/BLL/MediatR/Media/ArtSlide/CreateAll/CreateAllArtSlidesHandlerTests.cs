using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.MediatR.Media.ArtSlide.Create;
using Streetcode.BLL.MediatR.Media.ArtSlide.CreateAll;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.ArtSlide.CreateAll;

public class CreateAllArtSlidesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IStreetcodeRepository> _streetcodeRepoMock;
    private readonly Mock<IStreetcodeArtSlideRepository> _slideRepoMock;
    private readonly Mock<IArtSlideItemRepository> _slideItemRepoMock;
    private readonly IMapper _mapper;
    private readonly CreateAllArtSlidesHandler _handler;

    public CreateAllArtSlidesHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArtSlideProfile>();
        });

        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _streetcodeRepoMock = new Mock<IStreetcodeRepository>();
        _slideRepoMock = new Mock<IStreetcodeArtSlideRepository>();
        _slideItemRepoMock = new Mock<IArtSlideItemRepository>();

        _repoWrapperMock.Setup(r => r.StreetcodeRepository).Returns(_streetcodeRepoMock.Object);
        _repoWrapperMock.Setup(r => r.StreetcodeArtSlideRepository).Returns(_slideRepoMock.Object);
        _repoWrapperMock.Setup(r => r.ArtSlideItemRepository).Returns(_slideItemRepoMock.Object);

        _handler = new CreateAllArtSlidesHandler(_mapper, _repoWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSlideDto_WhenDataIsValid()
    {
        // Arrange
        var requestDto = new List<CreateStreetcodeArtSlideDto>
    {
        new() { StreetcodeId = 1, ArtSlideItems = new() { new() { ArtId = 1, Index = 0 } } }
    };
        var command = new CreateAllArtSlidesCommand(requestDto);

        _streetcodeRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync(new StreetcodeContent { Id = 1 });

        _repoWrapperMock.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _repoWrapperMock.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _slideRepoMock.Verify(r => r.CreateAsync(It.IsAny<StreetcodeArtSlide>()), Times.Once);
        _slideItemRepoMock.Verify(r => r.CreateAsync(It.IsAny<ArtSlideItem>()), Times.Once);

        _repoWrapperMock.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
    }
}