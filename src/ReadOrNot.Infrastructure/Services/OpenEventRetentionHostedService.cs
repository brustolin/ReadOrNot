using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Application.Options;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class OpenEventRetentionHostedService(
    IServiceProvider serviceProvider,
    IOptions<RetentionOptions> retentionOptions,
    ILogger<OpenEventRetentionHostedService> logger) : BackgroundService
{
    private readonly RetentionOptions _retentionOptions = retentionOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_retentionOptions.PurgeEnabled || _retentionOptions.OpenEventsRetentionDays <= 0)
        {
            logger.LogInformation("Open event retention cleanup is disabled.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var retentionService = scope.ServiceProvider.GetRequiredService<ITokenRetentionService>();
                await retentionService.PurgeOldEventsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while running open event retention cleanup.");
            }

            var delay = TimeSpan.FromHours(Math.Max(1, _retentionOptions.CleanupIntervalHours));
            await Task.Delay(delay, stoppingToken);
        }
    }
}
