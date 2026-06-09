using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.WebApi.Controllers.Streetcode.TextContent;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers.Streetcode.TextContent;

public class RelatedTermControllerTests
{
    private const int RelatedTermId = 1;
    private const int TermId = 2;
    private const string RelatedTermWord = "test";
    private const string UpdatedRelatedTermWord = "updated";

    private readonly Mock<IMediator> mediatorMock;
    private readonly RelatedTermController controller;

    public RelatedTermControllerTests()
    {
        mediatorMock = new Mock<IMediator>();
        controller = new RelatedTermController();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection()
                    .AddSingleton(mediatorMock.Object)
                    .BuildServiceProvider(),
            },
        };
    }

    [Fact]
    public async Task GetByTermId_ShouldReturnOk_WhenResultIsSuccess()
    {
        // Arrange
        var relatedTerms = new List<RelatedTermDTO>
        {
            CreateRelatedTermDto(),
        };

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetAllRelatedTermsByTermIdQuery>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok<IEnumerable<RelatedTermDTO>>(relatedTerms)));

        // Act
        var result = await controller.GetByTermId(TermId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(relatedTerms, okResult.Value);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenResultIsSuccess()
    {
        // Arrange
        var createRelatedTerm = new CreateRelatedTermDto
        {
            Word = RelatedTermWord,
            TermId = TermId,
        };
        var expectedResult = CreateRelatedTermDto();

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateRelatedTermCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedResult));

        // Act
        var result = await controller.Create(createRelatedTerm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenResultIsSuccess()
    {
        // Arrange
        var relatedTerm = CreateRelatedTermDto(UpdatedRelatedTermWord);

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateRelatedTermCommand>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(Unit.Value)));

        // Act
        var result = await controller.Update(RelatedTermId, relatedTerm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(Unit.Value, okResult.Value);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenResultIsSuccess()
    {
        // Arrange
        var deletedRelatedTerm = CreateRelatedTermDto();

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<DeleteRelatedTermCommand>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(deletedRelatedTerm)));

        // Act
        var result = await controller.Delete(RelatedTermWord, TermId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(deletedRelatedTerm, okResult.Value);
    }

    private static RelatedTermDTO CreateRelatedTermDto(string word = RelatedTermWord)
    {
        return new RelatedTermDTO
        {
            Id = RelatedTermId,
            Word = word,
            TermId = TermId,
        };
    }
}
