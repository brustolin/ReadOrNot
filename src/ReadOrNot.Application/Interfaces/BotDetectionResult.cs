namespace ReadOrNot.Application.Interfaces;

public sealed record BotDetectionResult(bool IsLikelyBot, string? Signals);
