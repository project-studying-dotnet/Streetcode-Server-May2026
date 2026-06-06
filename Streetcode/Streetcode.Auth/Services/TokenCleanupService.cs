using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Streetcode.Auth.Data;

namespace Streetcode.Auth.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task DoWork()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var expiredTokens = context.RefreshTokens
                    .Where(t => t.Expires < DateTime.UtcNow);

                if (expiredTokens.Any())
                {
                    context.RefreshTokens.RemoveRange(expiredTokens);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}