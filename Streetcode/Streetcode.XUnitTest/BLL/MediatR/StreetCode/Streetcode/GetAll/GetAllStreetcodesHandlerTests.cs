using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAll;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specifications.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.GetAll;

public class GetAllStreetcodesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllStreetcodesHandler _handler;

    public GetAllStreetcodesHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetAllStreetcodesHandler(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectResponse()
    {
        var requestDto = new GetAllStreetcodesRequestDTO
        {
            Page = 1,
            Amount = 10
        };
        var query = new GetAllStreetcodesQuery(requestDto);

        var streetcodesList = new List<StreetcodeContent>
        {
            new StreetcodeContent(),
            new StreetcodeContent()
        };

        _mockRepo.Setup(x => x.StreetcodeRepository.FindAll(It.IsAny<Expression<Func<StreetcodeContent, bool>>>()))
            .Returns(streetcodesList.AsQueryable());

        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<ISpecification<StreetcodeContent>>()))
            .ReturnsAsync(streetcodesList);

        var dtoList = new List<StreetcodeDTO>
        {
            new StreetcodeDTO(),
            new StreetcodeDTO()
        };

        _mockMapper.Setup(x => x.Map<IEnumerable<StreetcodeDTO>>(It.IsAny<IEnumerable<StreetcodeContent>>()))
            .Returns(dtoList);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Pages.Should().Be(1);
        result.Value.Streetcodes.Should().HaveCount(2);
    }
}