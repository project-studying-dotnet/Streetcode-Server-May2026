using MassTransit;
using Microsoft.Extensions.Options;
using Streetcode.Auth.Models;

namespace Streetcode.Auth.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMqSettings>(config.GetSection("RabbitMq"));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var options = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                    cfg.Host(options.Host, options.VirtualHost, h =>
                    {
                        h.Username(options.UserName);
                        h.Password(options.Password);
                    });
                });
            });

            return services;
        }
    }
}
