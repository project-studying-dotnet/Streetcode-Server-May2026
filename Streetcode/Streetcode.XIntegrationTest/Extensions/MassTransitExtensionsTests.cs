using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.Auth.Extensions;
using Xunit;

namespace Streetcode.XIntegrationTest.Extensions
{
    public class MassTransitExtensionsTests
    {
        [Fact]
        public void AddRabbitMq_ShouldRegisterMassTransitAndConfigureSettings()
        {
            // 1. Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                {"RabbitMq:Host", "localhost"},
                {"RabbitMq:VirtualHost", "/"},
                {"RabbitMq:UserName", "guest"},
                {"RabbitMq:Password", "guest"}
                }!)
                .Build();

            // 2. Act
            services.AddRabbitMq(configuration);
            var provider = services.BuildServiceProvider();

            // 3. Assert
            provider.GetService<IBusControl>().Should().NotBeNull();
            provider.GetService<IBus>().Should().NotBeNull();

            var options = provider.GetService<Microsoft.Extensions.Options.IOptions<Streetcode.Auth.Models.RabbitMqSettings>>();
            options.Should().NotBeNull();
            options!.Value.Host.Should().Be("localhost");
        }
    }
}
