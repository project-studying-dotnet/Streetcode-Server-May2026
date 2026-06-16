using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.Models.Entities;
using Xunit;
using FluentAssertions;

namespace Streetcode.XUnitTest.AuthService.Extensions
{
    public class AuthSeederTests
    {
        private readonly Mock<UserManager<User>> _mockUserMgr;
        private readonly Mock<RoleManager<IdentityRole<int>>> _mockRoleMgr;
        private readonly IConfiguration _config;

        public AuthSeederTests()
        {
            _mockUserMgr = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _mockRoleMgr = new Mock<RoleManager<IdentityRole<int>>>(Mock.Of<IRoleStore<IdentityRole<int>>>(), null!, null!, null!, null!);

            _config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"AdminSettings:Email", "admin@test.com"},
                {"AdminSettings:Password", "Password123!"}
            }
            !).Build();
        }

        [Fact]
        public async Task SeedAsync_ShouldCreateAdmin_WhenEverythingIsCorrect()
        {
            // Arrange
            _mockUserMgr.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);
            _mockUserMgr.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockRoleMgr.Setup(m => m.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockUserMgr.Setup(m => m.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);
            _mockUserMgr.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await AuthSeeder.SeedAsync(_mockUserMgr.Object, _mockRoleMgr.Object, _config);

            // Assert
            _mockUserMgr.Verify(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SeedAsync_ShouldThrowException_WhenUserCreationFails()
        {
            // Arrange
            var error = new IdentityError { Description = "Password too weak" };
            _mockUserMgr.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);
            _mockUserMgr.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Failed(error));

            // Act & Assert
            var act = () => AuthSeeder.SeedAsync(_mockUserMgr.Object, _mockRoleMgr.Object, _config);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Password too weak*");
        }
    }
}