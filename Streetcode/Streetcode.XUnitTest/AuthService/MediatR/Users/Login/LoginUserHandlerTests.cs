using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.Login
{
    public class LoginUserHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly LoginUserHandler _handler;

        public LoginUserHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>()).CreateMapper();
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            _loggerMock = new Mock<ILoggerService>();
            _authServiceMock = new Mock<IAuthService>();

            _handler = new LoginUserHandler(_userManagerMock.Object, _mapper, _loggerMock.Object, _authServiceMock.Object);
        }

        private static User CreateTestUser() => new()
        {
            Id = 1,
            UserName = "testUser",
            Name = "John",
            Surname = "Doe"
        };

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsSuccessResult()
        {
            var user = CreateTestUser();
            var loginDto = new UserLoginDto { Login = user.UserName!, Password = "password" };

            var authResponse = new AuthResponseDto
            {
                User = new UserDto { Id = user.Id, Name = user.Name, Surname = user.Surname },
                Token = "jwt-token",
                RefreshToken = "refresh-token",
                ExpireAt = DateTime.UtcNow.AddHours(1)
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Login!)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
            _authServiceMock.Setup(x => x.CreateLoginResultAsync(user)).ReturnsAsync(authResponse);

            var result = await _handler.Handle(new LoginUserCommand(loginDto), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(authResponse);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailResult()
        {
            var loginDto = new UserLoginDto { Login = "nonexistent", Password = "password" };

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            var result = await _handler.Handle(new LoginUserCommand(loginDto), CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == "Invalid login or password.");
            _loggerMock.Verify(x => x.LogError(It.IsAny<LoginUserCommand>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidPassword_ReturnsFailResult()
        {
            var user = CreateTestUser();
            var loginDto = new UserLoginDto { Login = user.UserName!, Password = "wrongPassword" };

            _userManagerMock.Setup(x => x.FindByNameAsync(user.UserName!)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(false);

            var result = await _handler.Handle(new LoginUserCommand(loginDto), CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            _loggerMock.Verify(x => x.LogError(It.IsAny<LoginUserCommand>(), It.IsAny<string>()), Times.Once);
            _authServiceMock.Verify(x => x.CreateLoginResultAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SuccessfulLogin_LogsInformation()
        {
            var user = CreateTestUser();
            var loginDto = new UserLoginDto { Login = user.UserName!, Password = "password" };

            _userManagerMock.Setup(x => x.FindByNameAsync(user.UserName!)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);

            await _handler.Handle(new LoginUserCommand(loginDto), CancellationToken.None);

            _loggerMock.Verify(x => x.LogInformation(It.Is<string>(s => s.Contains("successfully logged in"))), Times.Once);
        }
    }
}