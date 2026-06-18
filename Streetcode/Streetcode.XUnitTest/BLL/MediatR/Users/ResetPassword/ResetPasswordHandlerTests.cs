using Moq;
using Xunit;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.MediatR.Users.ResetPassword;
using Streetcode.DAL.Entities.Users;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.XUnitTest.BLL.MediatR.Users.ResetPassword
{
    public class ResetPasswordHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly ResetPasswordHandler _handler;

        public ResetPasswordHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            _handler = new ResetPasswordHandler(_userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFail()
        {
            // Arrange
            var dto = new ResetPasswordDto { Email = "test@test.com", Token = "tok", NewPassword = "Pass" };
            var command = new ResetPasswordCommand(dto);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains("Invalid request or user not found.", result.Errors.Select(e => e.Message));
        }

        [Fact]
        public async Task Handle_ResetSucceeded_ReturnsOk()
        {
            // Arrange
            var user = new User { Email = "test@test.com", Name = "Test", Surname = "User" };
            var dto = new ResetPasswordDto { Email = "test@test.com", Token = "tok", NewPassword = "NewPass" };
            var command = new ResetPasswordCommand(dto);

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, dto.Token, dto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ResetFailed_ReturnsFail()
        {
            // Arrange
            var user = new User { Email = "test@test.com", Name = "Test", Surname = "User" };
            var dto = new ResetPasswordDto { Email = "test@test.com", Token = "tok", NewPassword = "NewPass" };
            var command = new ResetPasswordCommand(dto);

            var error = new IdentityError { Description = "Invalid Token" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, dto.Token, dto.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains("Invalid Token", result.Errors.Select(e => e.Message));
        }
    }
}
