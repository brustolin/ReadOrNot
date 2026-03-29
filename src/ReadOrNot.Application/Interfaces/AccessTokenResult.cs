namespace ReadOrNot.Application.Interfaces;

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
