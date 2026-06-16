using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Streetcode.Auth.Data;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Enums;
using Xunit;

namespace Streetcode.XIntegrationTest.AuthService.MediatR
{
    public class LoginUserHandlerTests : IAsyncLifetime
    {
        private ServiceProvider _provider = null!;

        public Task InitializeAsync()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddLogging();

            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            _provider = services.BuildServiceProvider();

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _provider.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ShouldReturnLoginResult_WhenCredentialsAreValid()
        {
            // Arrange
            var userManager = _provider.GetRequiredService<UserManager<User>>();

            var user = new User
            {
                Email = "test@test.com",
                UserName = "test@test.com",
                Name = "Test",
                Surname = "User",
                Role = UserRole.Moderator
            };

            var createResult = await userManager.CreateAsync(user, "Password123");

            Assert.True(
                createResult.Succeeded,
                string.Join(", ", createResult.Errors.Select(e => e.Description)));

            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILoggerService>();
            var authService = new Mock<IAuthService>();

            mapper.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
                .Returns((User u) => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email!
                });

            authService.Setup(x => x.CreateLoginResultAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => new AuthResponseDto
                {
                    User = new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email!
                    },
                    Token = "jwt_token",
                    RefreshToken = "refresh_token",
                    ExpireAt = DateTime.UtcNow.AddMinutes(10)
                });

            var handler = new LoginUserHandler(
                userManager,
                mapper.Object,
                logger.Object,
                authService.Object);

            var command = new LoginUserCommand(
                new UserLoginDto
                {
                    Login = "test@test.com",
                    Password = "Password123"
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("jwt_token", result.Value.Token);
            Assert.Equal("refresh_token", result.Value.RefreshToken);
            Assert.Equal("test@test.com", result.Value.User.Email);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenPasswordIsIncorrect()
        {
            // Arrange
            var userManager = _provider.GetRequiredService<UserManager<User>>();

            var user = new User
            {
                Email = "test@test.com",
                UserName = "test@test.com",
                Name = "Test",
                Surname = "User",
                Role = UserRole.Moderator
            };

            await userManager.CreateAsync(user, "Password123");

            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILoggerService>();
            var authService = new Mock<IAuthService>();

            var handler = new LoginUserHandler(
                userManager,
                mapper.Object,
                logger.Object,
                authService.Object);

            var command = new LoginUserCommand(
                new UserLoginDto
                {
                    Login = "test@test.com",
                    Password = "WrongPassword"
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            authService.Verify(
                x => x.CreateLoginResultAsync(It.IsAny<User>()),
                Times.Never);
        }
    }
}