using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.MediatR.Media.ArtSlide.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;
using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.ArtSlide.Create;

public class CreateArtSlideHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IStreetcodeRepository> _streetcodeRepoMock;
    private readonly Mock<IArtRepository> _artRepoMock;
    private readonly Mock<IStreetcodeArtSlideRepository> _slideRepoMock;
    private readonly Mock<IArtSlideItemRepository> _slideItemRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateArtSlideHandler _handler;

    public CreateArtSlideHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArtSlideProfile>();
        });

        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _streetcodeRepoMock = new Mock<IStreetcodeRepository>();
        _artRepoMock = new Mock<IArtRepository>();
        _slideRepoMock = new Mock<IStreetcodeArtSlideRepository>();
        _slideItemRepoMock = new Mock<IArtSlideItemRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repoWrapperMock.Setup(r => r.StreetcodeRepository).Returns(_streetcodeRepoMock.Object);
        _repoWrapperMock.Setup(r => r.ArtRepository).Returns(_artRepoMock.Object);
        _repoWrapperMock.Setup(r => r.StreetcodeArtSlideRepository).Returns(_slideRepoMock.Object);
        _repoWrapperMock.Setup(r => r.ArtSlideItemRepository).Returns(_slideItemRepoMock.Object);

        _handler = new CreateArtSlideHandler(_mapper, _repoWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSlideDto_WhenDataIsValid()
    {
        // Arrange
        var requestDto = CreateRequestDto();
        var command = new CreateArtSlideCommand(requestDto);

        _streetcodeRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync(new StreetcodeContent { Id = requestDto.StreetcodeId });

        var arts = new List<ArtEntity> { new() { Id = 1 } }.AsQueryable().BuildMock();
        _artRepoMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), null))
            .ReturnsAsync(arts.ToList());

        _artRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), null))
            .ReturnsAsync(new ArtEntity { Id = 1 });

        // Настройка транзакции (обязательно для метода Handle)
        _repoWrapperMock.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _repoWrapperMock.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _repoWrapperMock.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        _slideRepoMock.Verify(r => r.Create(It.IsAny<StreetcodeArtSlide>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenStreetcodeDoesNotExist()
    {
        // Arrange
        var requestDto = CreateRequestDto();
        var command = new CreateArtSlideCommand(requestDto);

        _streetcodeRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync((StreetcodeContent)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains(string.Format(ErrorMessages.StreetcodeNotFound, requestDto.StreetcodeId)));
    }

    private static CreateStreetcodeArtSlideDto CreateRequestDto() => new()
    {
        StreetcodeId = 1,
        TemplateId = 1,
        ArtSlideItems = new List<ArtSlideItemDto> { new() { ArtId = 1, Index = 0 } }
    };
}