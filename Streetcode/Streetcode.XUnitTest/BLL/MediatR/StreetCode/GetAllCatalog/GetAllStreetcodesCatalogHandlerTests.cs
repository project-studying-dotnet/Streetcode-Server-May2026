using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specifications.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.GetAllCatalog;

public class GetAllStreetcodesCatalogHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllStreetcodesCatalogHandler _handler;

    public GetAllStreetcodesCatalogHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetAllStreetcodesCatalogHandler(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_StreetcodesNotNull_ReturnsOkResult()
    {
        var query = new GetAllStreetcodesCatalogQuery(1, 10);
        var streetcodes = new List<StreetcodeContent> { new StreetcodeContent() };
        var dtos = new List<RelatedFigureDTO> { new RelatedFigureDTO() };

        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<ISpecification<StreetcodeContent>>()))
            .ReturnsAsync(streetcodes);

        _mockMapper.Setup(x => x.Map<IEnumerable<RelatedFigureDTO>>(It.IsAny<IEnumerable<StreetcodeContent>>()))
            .Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task Handle_StreetcodesNull_ReturnsFailResult()
    {
        var query = new GetAllStreetcodesCatalogQuery(1, 10);

        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<ISpecification<StreetcodeContent>>()))
            .ReturnsAsync((IEnumerable<StreetcodeContent>)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Cannot find any subtitles");
        _mockLogger.Verify(x => x.LogError(query, "Cannot find any subtitles"), Times.Once);
    }
}