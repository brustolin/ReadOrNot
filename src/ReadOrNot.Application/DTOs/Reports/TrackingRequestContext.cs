namespace ReadOrNot.Application.DTOs.Reports;

public sealed record TrackingRequestContext(
    string PublicIdentifier,
    string? IpAddress,
    string? UserAgent,
    string? Referer,
    string? AcceptLanguage,
    string? QueryString,
    string HttpMethod);
