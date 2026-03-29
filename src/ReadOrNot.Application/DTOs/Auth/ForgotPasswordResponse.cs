namespace ReadOrNot.Application.DTOs.Auth;

public sealed record ForgotPasswordResponse(bool Accepted, string? DevelopmentResetToken, string? DevelopmentResetUrl);
