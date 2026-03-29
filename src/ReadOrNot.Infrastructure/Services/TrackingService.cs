using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReadOrNot.Application.DTOs.Reports;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Domain.Entities;
using ReadOrNot.Infrastructure.Persistence;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class TrackingService(
    ReadOrNotDbContext dbContext,
    IIpPrivacyService ipPrivacyService,
    IBotDetector botDetector,
    IClock clock,
    ILogger<TrackingService> logger) : ITrackingService
{
    public async Task<TrackingHitResult> TrackAsync(TrackingRequestContext request, CancellationToken cancellationToken)
    {
        var token = await dbContext.TrackingTokens
            .SingleOrDefaultAsync(token => token.PublicIdentifier == request.PublicIdentifier, cancellationToken);

        if (token is null)
        {
            logger.LogInformation("Tracking request received for unknown token '{PublicIdentifier}'.", request.PublicIdentifier);
            return new TrackingHitResult(false, "invalid-token", false);
        }

        if (!token.IsEnabled)
        {
            return new TrackingHitResult(false, "disabled-token", false);
        }

        if (token.IsExpired(clock.UtcNow))
        {
            return new TrackingHitResult(false, "expired-token", false);
        }

        var botDetection = botDetector.Analyze(request.UserAgent, request.AcceptLanguage, request.QueryString);
        var processedIp = ipPrivacyService.Process(request.IpAddress, request.UserAgent, request.AcceptLanguage);
        var openEvent = new OpenEvent(
            token.Id,
            clock.UtcNow,
            processedIp.StoredIpAddress,
            processedIp.StoredIpHash,
            TrimToLength(request.UserAgent, 1024),
            TrimToLength(request.Referer, 1024),
            TrimToLength(request.AcceptLanguage, 256),
            TrimToLength(request.QueryString, 2048),
            processedIp.VisitorFingerprintHash,
            botDetection.IsLikelyBot,
            TrimToLength(botDetection.Signals, 512),
            TrimToLength(request.HttpMethod, 16));

        dbContext.OpenEvents.Add(openEvent);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TrackingHitResult(true, "tracked", botDetection.IsLikelyBot);
    }

    private static string? TrimToLength(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length <= maxLength ? normalized : normalized[..maxLength];
    }
}
