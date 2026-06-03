using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Users.Logout;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Users.Logout;

public class LogoutUserHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly LogoutUserHandler _handler;

    public LogoutUserHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();

        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<User>>(),
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<User>>>());

        _loggerMock = new Mock<ILoggerService>();

        _handler = new LogoutUserHandler(
            _userManagerMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldClearRefreshToken_WhenUserExists()
    {
        const int userId = 1;

        var user = new User
        {
            Id = userId,
            UserName = "admin",
            Name = "Admin",
            Surname = "User",
            Email = "admin@example.com",
            Role = UserRole.MainAdministrator,
            RefreshToken = "refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
        };

        _userManagerMock
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _handler.Handle(new LogoutUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiryTime.Should().BeNull();

        _userManagerMock.Verify(m => m.FindByIdAsync(userId.ToString()), Times.Once);
        _userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserDoesNotExist()
    {
        const int userId = 99;

        _userManagerMock
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(new LogoutUserCommand(userId), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(error => error.Message.Contains("not found"));

        _userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
