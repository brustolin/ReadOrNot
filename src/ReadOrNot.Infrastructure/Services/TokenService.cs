using Microsoft.EntityFrameworkCore;
using ReadOrNot.Application.Common;
using ReadOrNot.Application.DTOs.Reports;
using ReadOrNot.Application.DTOs.Tokens;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Domain.Entities;
using ReadOrNot.Infrastructure.Persistence;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class TokenService(
    ReadOrNotDbContext dbContext,
    ITokenIdentifierGenerator tokenIdentifierGenerator,
    ITokenPublicUrlBuilder tokenPublicUrlBuilder,
    IClock clock) : ITokenService
{
    public async Task<IReadOnlyCollection<TrackingTokenListItemDto>> GetTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await dbContext.TrackingTokens
            .AsNoTracking()
            .Where(token => token.UserId == userId)
            .OrderByDescending(token => token.CreatedAtUtc)
            .Select(token => new
            {
                Token = token,
                TotalOpens = dbContext.OpenEvents.Count(openEvent => openEvent.TokenId == token.Id),
                LastOpenUtc = dbContext.OpenEvents
                    .Where(openEvent => openEvent.TokenId == token.Id)
                    .OrderByDescending(openEvent => openEvent.OccurredAtUtc)
                    .Select(openEvent => (DateTime?)openEvent.OccurredAtUtc)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return tokens
            .Select(item => MapListItem(item.Token, item.TotalOpens, item.LastOpenUtc))
            .ToArray();
    }

    public async Task<TrackingTokenDetailsDto> GetTokenAsync(Guid userId, int tokenId, TokenReportQuery query, CancellationToken cancellationToken)
    {
        ValidateReportQuery(query);

        var token = await GetOwnedTokenAsync(userId, tokenId, cancellationToken);
        var report = await BuildReportAsync(tokenId, query, cancellationToken);
        return MapDetails(token, report);
    }

    public async Task<TrackingTokenDetailsDto> CreateTokenAsync(Guid userId, CreateTrackingTokenRequest request, CancellationToken cancellationToken)
    {
        var expiresAtUtc = NormalizeExpiry(request.ExpiresAtUtc);
        var nowUtc = clock.UtcNow;
        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= nowUtc)
        {
            throw CreateExpiryValidationException();
        }

        var token = new TrackingToken(
            userId,
            tokenIdentifierGenerator.Create(),
            request.Name.Trim(),
            NormalizeDescription(request.Description),
            nowUtc,
            expiresAtUtc);

        dbContext.TrackingTokens.Add(token);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapDetails(token, new TokenReportDto(0, 0, null, null, [], []));
    }

    public async Task<TrackingTokenDetailsDto> UpdateTokenAsync(Guid userId, int tokenId, UpdateTrackingTokenRequest request, CancellationToken cancellationToken)
    {
        var token = await GetOwnedTokenAsync(userId, tokenId, cancellationToken);
        var expiresAtUtc = NormalizeExpiry(request.ExpiresAtUtc);
        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= clock.UtcNow)
        {
            throw CreateExpiryValidationException();
        }

        token.UpdateDetails(request.Name.Trim(), NormalizeDescription(request.Description), expiresAtUtc);
        token.SetEnabled(request.IsEnabled);

        await dbContext.SaveChangesAsync(cancellationToken);

        var report = await BuildReportAsync(tokenId, new TokenReportQuery(), cancellationToken);
        return MapDetails(token, report);
    }

    public async Task SetTokenEnabledAsync(Guid userId, int tokenId, bool isEnabled, CancellationToken cancellationToken)
    {
        var token = await GetOwnedTokenAsync(userId, tokenId, cancellationToken);
        token.SetEnabled(isEnabled);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTokenAsync(Guid userId, int tokenId, CancellationToken cancellationToken)
    {
        var token = await GetOwnedTokenAsync(userId, tokenId, cancellationToken);
        dbContext.TrackingTokens.Remove(token);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<TrackingToken> GetOwnedTokenAsync(Guid userId, int tokenId, CancellationToken cancellationToken)
    {
        return await dbContext.TrackingTokens
            .SingleOrDefaultAsync(token => token.UserId == userId && token.Id == tokenId, cancellationToken)
            ?? throw new NotFoundException("Tracking token was not found.");
    }

    private async Task<TokenReportDto> BuildReportAsync(int tokenId, TokenReportQuery query, CancellationToken cancellationToken)
    {
        var openEventsQuery = dbContext.OpenEvents
            .AsNoTracking()
            .Where(openEvent => openEvent.TokenId == tokenId);

        if (query.FromUtc.HasValue)
        {
            openEventsQuery = openEventsQuery.Where(openEvent => openEvent.OccurredAtUtc >= query.FromUtc.Value);
        }

        if (query.ToUtc.HasValue)
        {
            openEventsQuery = openEventsQuery.Where(openEvent => openEvent.OccurredAtUtc <= query.ToUtc.Value);
        }

        if (!query.IncludeLikelyBots)
        {
            openEventsQuery = openEventsQuery.Where(openEvent => !openEvent.IsLikelyBot);
        }

        var openEvents = await openEventsQuery
            .OrderByDescending(openEvent => openEvent.OccurredAtUtc)
            .ToListAsync(cancellationToken);

        var uniqueOpenCount = openEvents
            .Select(openEvent => openEvent.VisitorFingerprintHash)
            .Where(hash => !string.IsNullOrWhiteSpace(hash))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        var dailyCounts = openEvents
            .GroupBy(openEvent => DateOnly.FromDateTime(openEvent.OccurredAtUtc))
            .OrderBy(group => group.Key)
            .Select(group => new ChartPointDto(group.Key, group.Count()))
            .ToArray();

        var orderedChronologicalEvents = openEvents
            .OrderBy(openEvent => openEvent.OccurredAtUtc)
            .Select(openEvent => new OpenEventDto(
                openEvent.Id,
                openEvent.OccurredAtUtc,
                openEvent.IpAddress,
                openEvent.IpAddressHash,
                openEvent.UserAgent,
                openEvent.Referer,
                openEvent.AcceptLanguage,
                openEvent.QueryString,
                openEvent.IsLikelyBot,
                openEvent.BotSignals,
                openEvent.HttpMethod))
            .ToArray();

        return new TokenReportDto(
            openEvents.Count,
            uniqueOpenCount,
            orderedChronologicalEvents.FirstOrDefault()?.OccurredAtUtc,
            orderedChronologicalEvents.LastOrDefault()?.OccurredAtUtc,
            dailyCounts,
            orderedChronologicalEvents);
    }

    private TrackingTokenListItemDto MapListItem(TrackingToken token, int totalOpens, DateTime? lastOpenUtc)
    {
        var trackingUrl = tokenPublicUrlBuilder.BuildTrackingUrl(token.PublicIdentifier);
        return new TrackingTokenListItemDto(
            token.Id,
            token.PublicIdentifier,
            token.Name,
            token.Description,
            token.CreatedAtUtc,
            token.ExpiresAtUtc,
            token.IsEnabled,
            trackingUrl,
            BuildSuggestedImgTag(trackingUrl),
            totalOpens,
            lastOpenUtc);
    }

    private TrackingTokenDetailsDto MapDetails(TrackingToken token, TokenReportDto report)
    {
        var trackingUrl = tokenPublicUrlBuilder.BuildTrackingUrl(token.PublicIdentifier);
        return new TrackingTokenDetailsDto(
            token.Id,
            token.PublicIdentifier,
            token.Name,
            token.Description,
            token.CreatedAtUtc,
            token.ExpiresAtUtc,
            token.IsEnabled,
            trackingUrl,
            BuildSuggestedImgTag(trackingUrl),
            report);
    }

    private static string BuildSuggestedImgTag(string trackingUrl)
    {
        return $"<img src=\"{trackingUrl}\" alt=\"\" width=\"1\" height=\"1\" style=\"display:block\" />";
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static DateTime? NormalizeExpiry(DateTime? expiresAtUtc)
    {
        return expiresAtUtc?.ToUniversalTime();
    }

    private static void ValidateReportQuery(TokenReportQuery query)
    {
        if (query.FromUtc.HasValue && query.ToUtc.HasValue && query.FromUtc.Value > query.ToUtc.Value)
        {
            throw new ValidationAppException(
                "Invalid report filters.",
                new Dictionary<string, string[]> { ["dateRange"] = ["The start date must be earlier than or equal to the end date."] });
        }
    }

    private static ValidationAppException CreateExpiryValidationException()
    {
        return new ValidationAppException(
            "Invalid expiration date.",
            new Dictionary<string, string[]> { ["expiresAtUtc"] = ["Expiration must be in the future."] });
    }
}
