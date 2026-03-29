using ReadOrNot.Application.DTOs.Reports;

namespace ReadOrNot.Application.DTOs.Tokens;

public sealed record TrackingTokenDetailsDto(
    int Id,
    string PublicIdentifier,
    string Name,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime? ExpiresAtUtc,
    bool IsEnabled,
    string TrackingImageUrl,
    string SuggestedImgTag,
    TokenReportDto Report);
