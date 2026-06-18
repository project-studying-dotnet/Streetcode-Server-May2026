using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Streetcode.Auth.Extensions;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.Extensions
{
    public class ResultExtensionsTests
    {
        private class TestController : ControllerBase { }

        [Fact]
        public void ToActionResult_ShouldReturnOk_WhenResultIsSuccess()
        {
            // Arrange
            var controller = new TestController();
            var result = Result.Ok("Success data");

            // Act
            var actionResult = controller.ToActionResult(result);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("Success data");
        }

        [Fact]
        public void ToActionResult_ShouldReturnBadRequest_WhenResultIsFailed()
        {
            // Arrange
            var controller = new TestController();
            var result = Result.Fail<string>("Some error");

            // Act
            var actionResult = controller.ToActionResult(result);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Some error");
        }

        [Fact]
        public void ToActionResult_ShouldReturnStatusCode500_WhenResultIsNull()
        {
            // Arrange
            var controller = new TestController();
            Result<string>? result = null;

            // Act
            var actionResult = controller.ToActionResult(result!);

            // Assert
            actionResult.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }

        [Fact]
        public void ToActionResult_ShouldReturnNotFound_WhenResultIsSuccessButValueIsNull()
        {
            // Arrange
            var controller = new TestController();
            Result<string> result = Result.Ok<string>(null!);

            // Act
            var actionResult = controller.ToActionResult(result);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void ToActionResult_ShouldReturnBadRequestWithDefaultMessage_WhenResultIsFailedAndErrorIsNull()
        {
            // Arrange
            var controller = new TestController();

            var error = new Error(null!);
            var result = Result.Fail<string>(error);

            // Act
            var actionResult = controller.ToActionResult(result);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Something went wrong");
        }
    }
}
