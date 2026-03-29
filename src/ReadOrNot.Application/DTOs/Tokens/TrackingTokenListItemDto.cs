namespace ReadOrNot.Application.DTOs.Tokens;

public sealed record TrackingTokenListItemDto(
    int Id,
    string PublicIdentifier,
    string Name,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime? ExpiresAtUtc,
    bool IsEnabled,
    string TrackingImageUrl,
    string SuggestedImgTag,
    int TotalOpens,
    DateTime? LastOpenUtc);
