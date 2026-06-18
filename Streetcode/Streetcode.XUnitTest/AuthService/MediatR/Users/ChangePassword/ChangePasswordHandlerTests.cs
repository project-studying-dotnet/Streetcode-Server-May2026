using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.Auth.Models.DTO.Users;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Models.MediatR.Users.ChangePassword;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.ChangePassword
{
    public class ChangePasswordHandlerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly ChangePasswordHandler _handler;

        public ChangePasswordHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _handler = new ChangePasswordHandler(_mockUserManager.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var command = new ChangePasswordCommand(1, new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmNewPassword = "NewPassword123"
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("User not found", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccess()
        {
            // Arrange
            var user = new User { Id = 1, Name = "Test", Surname = "User" };

            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.ChangePasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var command = new ChangePasswordCommand(1, new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmNewPassword = "NewPassword123"
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}