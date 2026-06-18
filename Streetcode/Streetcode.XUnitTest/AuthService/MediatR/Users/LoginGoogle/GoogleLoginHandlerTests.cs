using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.Auth.MediatR.Users.LoginGoogle;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.LoginGoogle
{
    public class GoogleLoginHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GoogleLoginHandler _handler;

        public GoogleLoginHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GoogleLoginHandler(
                _userManagerMock.Object,
                _authServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ExistingUser_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                Email = "test@test.com",
                Id = 1,
                Name = "Test",
                Surname = "User"
            };
            var request = new GoogleLoginCommand(new GoogleLoginRequestDto { Email = "test@test.com" });

            _userManagerMock.Setup(x => x.FindByEmailAsync(request.googleLoginRequest.Email))
                .ReturnsAsync(user);

            _authServiceMock.Setup(x => x.CreateLoginResultAsync(user))
                .ReturnsAsync(new AuthResponseDto
                {
                    Token = "jwt",
                    RefreshToken = "refresh",
                    User = new UserDto { Id = 1 }
                });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("refresh", result.Value.RefreshToken);
            _authServiceMock.Verify(x => x.CreateLoginResultAsync(user), Times.Once);
        }

        [Fact]
        public async Task Handle_NewUser_CreatesAndAssignsRole()
        {
            // Arrange
            var request = new GoogleLoginCommand(new GoogleLoginRequestDto { Email = "new@test.com" });

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _authServiceMock.Setup(x => x.CreateLoginResultAsync(It.IsAny<User>()))
                .ReturnsAsync(new AuthResponseDto
                {
                    Token = "jwt",
                    RefreshToken = "refresh",
                    User = new UserDto { Id = 1 }
                });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
    }
}