namespace ReadOrNot.Application.DTOs.Reports;

public sealed record OpenEventDto(
    long Id,
    DateTime OccurredAtUtc,
    string? IpAddress,
    string? IpAddressHash,
    string? UserAgent,
    string? Referer,
    string? AcceptLanguage,
    string? QueryString,
    bool IsLikelyBot,
    string? BotSignals,
    string? HttpMethod);
