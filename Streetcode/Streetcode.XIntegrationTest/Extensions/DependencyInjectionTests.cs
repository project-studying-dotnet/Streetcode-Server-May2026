
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.Services.Interfaces.Users;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Hosting;

namespace Streetcode.XIntegrationTest.Extensions
{

    public class DependencyInjectionTests
    {
        [Fact]
        public void AddInfrastructure_ShouldRegisterServicesCorrectly()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                {"Jwt:Key", "SuperSecretKeyMustBeAtLeast32CharactersLong!"},
                {"Jwt:Issuer", "TestIssuer"},
                {"ConnectionStrings:DefaultConnection", "Server=dummy;Database=dummy;Trusted_Connection=True;"}
                }!)
                .Build();

            services.AddInfrastructure(configuration);

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IJwtTokenService>().Should().NotBeNull();
            serviceProvider.GetService<IRefreshTokenService>().Should().NotBeNull();

            var hostedServices = serviceProvider.GetServices<IHostedService>();
            hostedServices.Should().Contain(s => s.GetType() == typeof(Streetcode.Auth.Services.TokenCleanupService));
        }
    }
}
