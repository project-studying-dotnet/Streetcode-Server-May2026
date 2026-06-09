using AutoMapper;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Streetcode.Auth.Data;
using Streetcode.Auth.MediatR.Users.Register;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Enums;
using Streetcode.Common.Events;
using Xunit;

namespace Streetcode.XIntegrationTest.AuthService.MediatR
{
    public class RegisterUserHandlerTests : IAsyncLifetime
    {
        private ServiceProvider _provider;

        public Task InitializeAsync()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMassTransitTestHarness();

            _provider = services.BuildServiceProvider();

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _provider.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ShouldRegisterUser_AndPublishEvent()
        {
            // Arrange
            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            try
            {
                var userManager = _provider.GetRequiredService<UserManager<User>>();
                var roleManager = _provider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                var publishEndpoint = harness.Bus;

                var roleName = UserRole.Moderator.ToString();

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }

                var logger = new Mock<ILoggerService>();
                var mapper = new Mock<IMapper>();
                var authService = new Mock<IAuthService>();

                authService
                    .Setup(x => x.CreateLoginResultAsync(It.IsAny<User>()))
                    .ReturnsAsync(new AuthResponseDto
                    {
                        User = new UserDto(),
                        Token = "mock_token",
                        RefreshToken = "mock_refresh"
                    });

                mapper
                    .Setup(x => x.Map<User>(It.IsAny<UserRegisterDto>()))
                    .Returns((UserRegisterDto dto) => new User
                    {
                        Email = dto.Email,
                        UserName = dto.Email,
                        Name = dto.Name,
                        Surname = dto.Surname
                    });

                var handler = new RegisterUserHandler(
                    userManager,
                    mapper.Object,
                    logger.Object,
                    harness.Bus,
                    authService.Object);

                var command = new RegisterUserCommand(
                    new UserRegisterDto
                    {
                        Email = "test@test.com",
                        Name = "TestName",
                        Surname = "TestSurname",
                        Password = "Password123",
                        PasswordConfirmation = "Password123"
                    });

                // Act
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.True(result.IsSuccess);

                var userInDb = await userManager.FindByEmailAsync("test@test.com");
                Assert.NotNull(userInDb);

                var inRole = await userManager.IsInRoleAsync(userInDb, roleName);
                Assert.True(inRole);

                Assert.True(await harness.Published.Any<UserRegisteredEvent>());
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}