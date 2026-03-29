namespace ReadOrNot.Domain.Entities;

public sealed class OpenEvent(
    int tokenId,
    DateTime occurredAtUtc,
    string? ipAddress,
    string? ipAddressHash,
    string? userAgent,
    string? referer,
    string? acceptLanguage,
    string? queryString,
    string? visitorFingerprintHash,
    bool isLikelyBot,
    string? botSignals,
    string? httpMethod)
{
    public long Id { get; private set; }

    public int TokenId { get; private set; } = tokenId;

    public TrackingToken Token { get; private set; } = null!;

    public DateTime OccurredAtUtc { get; private set; } = occurredAtUtc;

    public string? IpAddress { get; private set; } = ipAddress;

    public string? IpAddressHash { get; private set; } = ipAddressHash;

    public string? UserAgent { get; private set; } = userAgent;

    public string? Referer { get; private set; } = referer;

    public string? AcceptLanguage { get; private set; } = acceptLanguage;

    public string? QueryString { get; private set; } = queryString;

    public string? VisitorFingerprintHash { get; private set; } = visitorFingerprintHash;

    public bool IsLikelyBot { get; private set; } = isLikelyBot;

    public string? BotSignals { get; private set; } = botSignals;

    public string? HttpMethod { get; private set; } = httpMethod;

    public string? MetadataJson { get; private set; }
}
