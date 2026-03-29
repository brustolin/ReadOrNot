namespace ReadOrNot.Application.DTOs.Account;

public sealed record AccountProfileResponse(Guid UserId, string Email, string DisplayName, string? TimeZone, DateTime CreatedAtUtc);
