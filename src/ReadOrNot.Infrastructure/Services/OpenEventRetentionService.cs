using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Application.Options;
using ReadOrNot.Infrastructure.Persistence;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class OpenEventRetentionService(
    ReadOrNotDbContext dbContext,
    IClock clock,
    IOptions<RetentionOptions> retentionOptions,
    ILogger<OpenEventRetentionService> logger) : ITokenRetentionService
{
    private readonly RetentionOptions _retentionOptions = retentionOptions.Value;

    public async Task<int> PurgeOldEventsAsync(CancellationToken cancellationToken)
    {
        if (!_retentionOptions.PurgeEnabled || _retentionOptions.OpenEventsRetentionDays <= 0)
        {
            return 0;
        }

        var cutoff = clock.UtcNow.AddDays(-_retentionOptions.OpenEventsRetentionDays);
        var deletedCount = await dbContext.OpenEvents
            .Where(openEvent => openEvent.OccurredAtUtc < cutoff)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount > 0)
        {
            logger.LogInformation("Purged {DeletedCount} open events older than {CutoffUtc}.", deletedCount, cutoff);
        }

        return deletedCount;
    }
}
