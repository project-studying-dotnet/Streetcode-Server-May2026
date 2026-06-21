using Microsoft.Extensions.Configuration;
using Streetcode.Auth.Services.Users;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.Services
{
    public class GoogleAuthServiceTests
    {
        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnNull_WhenTokenIsInvalid()
        {
            // Arrange
            var myConfiguration = new Dictionary<string, string>
        {
            {"GoogleAuth:ClientId", "fake-client-id"}
        };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration!)
                .Build();

            var service = new GoogleAuthService(configuration);

            // Act
            var result = await service.ValidateTokenAsync("invalid-token-string");

            // Assert
            Assert.Null(result);
        }
    }
}
