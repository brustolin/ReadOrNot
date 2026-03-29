using ReadOrNot.Application.DTOs.Account;

namespace ReadOrNot.Application.DTOs.Auth;

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, AccountProfileResponse Account);
