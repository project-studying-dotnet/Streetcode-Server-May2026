using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Streetcode.Auth.Data;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.Models.Entities;
using Xunit;

namespace Streetcode.XIntegrationTest.Extensions
{
    public class SeedExtensionsTests
    {
        [Fact]
        public async Task SeedDataAsync_ShouldRunMigrationsAndSeed()
        {
            // 1. Arrange
            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);

            var mockUserMgr = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            var mockRoleMgr = new Mock<RoleManager<IdentityRole<int>>>(Mock.Of<IRoleStore<IdentityRole<int>>>(), null!, null!, null!, null!);

            mockRoleMgr.Setup(m => m.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            mockUserMgr.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);
            mockUserMgr.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mockUserMgr.Setup(m => m.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);
            mockUserMgr.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"AdminSettings:Email", "admin@test.com"},
                    {"AdminSettings:Password", "Password123!"}
                }
                !)
                .Build();

            services.AddSingleton(mockUserMgr.Object);
            services.AddSingleton(mockRoleMgr.Object);
            services.AddSingleton(dbContext);
            services.AddSingleton<IConfiguration>(config);

            var serviceProvider = services.BuildServiceProvider();
            var mockHost = new Mock<IHost>();
            mockHost.Setup(h => h.Services).Returns(serviceProvider);

            // 2. Act
            await SeedExtensions.SeedDataAsync(mockHost.Object);

            // 3. Asser
            mockUserMgr.Verify(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
    }
}