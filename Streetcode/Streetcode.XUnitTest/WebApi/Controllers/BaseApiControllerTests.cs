using System.Security.Claims;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.ResultVariations;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers
{
    public class BaseApiControllerTests
    {
        [Fact]
        public void GetUserRole_ShouldReturnRole_WhenUserIsInRole()
        {
            var controller = CreateController(UserRole.MainAdministrator);

            var role = controller.ExposedGetUserRole();

            role.Should().Be(UserRole.MainAdministrator);
        }

        [Fact]
        public void GetUserRole_ShouldReturnNull_WhenUserHasNoMatchingRole()
        {
            var controller = CreateController();

            var role = controller.ExposedGetUserRole();

            role.Should().BeNull();
        }

        [Fact]
        public void HandleResult_ShouldReturnOk_WhenResultIsSuccessfulWithValue()
        {
            var controller = CreateController();
            var result = Result.Ok("value");

            var actionResult = controller.ExposedHandleResult(result);

            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("value");
        }

        [Fact]
        public void HandleResult_ShouldReturnNotFound_WhenResultIsSuccessfulWithNullValue()
        {
            var controller = CreateController();
            var result = Result.Ok<string?>(null);

            var actionResult = controller.ExposedHandleResult(result);

            var notFoundResult = actionResult.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Found result matching null");
        }

        [Fact]
        public void HandleResult_ShouldReturnOk_WhenResultIsNullResult()
        {
            var controller = CreateController();
            NullResult<string?> result = new NullResult<string?>();

            var actionResult = controller.ExposedHandleResult(result);

            actionResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void HandleResult_ShouldReturnBadRequest_WhenResultIsFailed()
        {
            var controller = CreateController();
            var result = Result.Fail<string>("validation error");

            var actionResult = controller.ExposedHandleResult(result);

            actionResult.Should().BeOfType<BadRequestObjectResult>();
        }

        private static TestableBaseApiController CreateController(UserRole? role = null)
        {
            var claims = role is null
                ? Array.Empty<Claim>()
                : new[] { new Claim(ClaimTypes.Role, role.ToString() !) };

            var controller = new TestableBaseApiController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "TestAuth")),
                    },
                },
            };

            return controller;
        }

        private sealed class TestableBaseApiController : BaseApiController
        {
            public UserRole? ExposedGetUserRole() => GetUserRole();

            public ActionResult ExposedHandleResult<T>(Result<T> result) => HandleResult(result);
        }
    }
}
