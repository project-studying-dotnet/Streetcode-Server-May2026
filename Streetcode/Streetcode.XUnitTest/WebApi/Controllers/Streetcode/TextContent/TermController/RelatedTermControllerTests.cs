using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.WebApi.Controllers;
using Streetcode.WebApi.Controllers.Streetcode.TextContent;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers.Streetcode.TextContent
{
    public class RelatedTermControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TestableRelatedTermController _controller;

        public RelatedTermControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TestableRelatedTermController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetByTermId_ReturnsOkResult_WithData()
        {
            var expected = new List<RelatedTermDTO>
            {
                new() { Id = 1, Word = "Test" }
            };

            _mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<GetAllRelatedTermsByTermIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok<IEnumerable<RelatedTermDTO>>(expected));

            var result = await _controller.GetByTermId(1);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            okResult.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WhenSuccessful()
        {
            var request = new CreateRelatedTermDto
            {
                TermId = 1,
                Word = "New"
            };

            var response = new RelatedTermDTO
            {
                Id = 1,
                TermId = 1,
                Word = "New"
            };

            _mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<CreateRelatedTermCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(response));

            var result = await _controller.Create(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenMediatorReturnsFailedResult()
        {
            var dto = new RelatedTermDTO
            {
                Id = 1,
                Word = "Updated"
            };

            _mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<UpdateRelatedTermCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<Unit>("Error"));

            var result = await _controller.Update(1, dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenSuccessful()
        {
            var deletedDto = new RelatedTermDTO
            {
                Id = 1,
                Word = "TestWord"
            };

            _mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<DeleteRelatedTermCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(deletedDto));

            var result = await _controller.Delete("TestWord");

            result.Should().BeOfType<OkObjectResult>();

            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(deletedDto);
        }

        private sealed class TestableRelatedTermController : RelatedTermController
        {
            public TestableRelatedTermController(IMediator mediator)
            {
                typeof(BaseApiController)
                    .GetField(
                        "_mediator",
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic)!
                    .SetValue(this, mediator);
            }
        }
    }
}