using System.Security.Claims;
using Xunit;
using FluentResults;
using FluentAssertions;
using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Resources;
using Microsoft.AspNetCore.Http;
using Streetcode.WebApi.Controllers;
using Streetcode.BLL.MediatR.ResultVariations;

namespace Streetcode.XUnitTest.WebApi;

public sealed class BaseApiControllerTests
{
    #region Static
    private static TestableController CreateController(UserRole? role = null)
    {
        Claim[] claims = [];
        if (role is not null)
        {
            claims = [new Claim(ClaimTypes.Role, role.Value.ToString())];
        }
        return new TestableController()
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "TestAuth"))
                }
            }
        };
    }
    #endregion

    #region Instance
    [Fact]
    public void GetUserRole_ShouldReturnRole_WhenUserIsInRole()
    {
        // Arrange
        TestableController controller = BaseApiControllerTests.CreateController(UserRole.MainAdministrator);

        // Act
        UserRole? role = controller.ExposedGetUserRole();

        // Assert
        role.Should().Be(UserRole.MainAdministrator);
    }

    [Fact]
    public void GetUserRole_ShouldReturnNull_WhenUserHasNoMatchingRole()
    {
        // Arrange
        TestableController controller = BaseApiControllerTests.CreateController();

        // Act
        UserRole? role = controller.ExposedGetUserRole();

        // Assert
        role.Should().BeNull();
    }

    [Fact]
    public void HandleResult_ShouldReturnOk_WhenResultIsSuccessfulWithValue()
    {
        // Arrange
        const string test_str = "value";
        TestableController controller = BaseApiControllerTests.CreateController(UserRole.MainAdministrator);
        Result<string> result = Result.Ok(test_str);

        // Act
        ActionResult action_result = controller.ExposedHandleResult(result);

        // Assert
        OkObjectResult ok_result = action_result.Should().BeOfType<OkObjectResult>().Subject;
        ok_result.Value.Should().Be(test_str);
    }

    [Fact]
    public void HandleResult_ShouldReturnNotFound_WhenResultIsSuccessfulWithNullValue()
    {
        // Arrange
        TestableController controller = BaseApiControllerTests.CreateController(UserRole.MainAdministrator);
        Result<string?> result = Result.Ok<string?>(null);

        // Act
        ActionResult action_result = controller.ExposedHandleResult(result);

        // Assert
        NotFoundObjectResult not_found_result = action_result.Should().BeOfType<NotFoundObjectResult>().Subject;
        not_found_result.Value.Should().Be(ErrorMessages.FoundResultMatchingNull);
    }

    [Fact]
    public void HandleResult_ShouldReturnOk_WhenResultIsNullResult()
    {
        // Arrange
        TestableController controller = BaseApiControllerTests.CreateController(UserRole.MainAdministrator);
        NullResult<string?> result = new();

        // Act
        ActionResult action_result = controller.ExposedHandleResult(result);

        // Assert
        action_result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void HandleResult_ShouldReturnBadRequest_WhenResultIsFailed()
    {
        // Arrange
        TestableController controller = BaseApiControllerTests.CreateController(UserRole.MainAdministrator);
        Result<string?> result = Result.Fail("Validation error");

        // Act
        ActionResult action_result = controller.ExposedHandleResult(result);

        // Assert
        action_result.Should().BeOfType<BadRequestObjectResult>();
    }
    #endregion

    #region Nested
    private sealed class TestableController : BaseApiController
    {
        public UserRole? ExposedGetUserRole()
        {
            return base.GetUserRole();
        }
        public ActionResult ExposedHandleResult<T>(Result<T> result)
        {
            return base.HandleResult(result);
        }
    }
    #endregion
}