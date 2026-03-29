namespace ReadOrNot.Application.DTOs.Reports;

public sealed record TrackingHitResult(bool Logged, string Outcome, bool IsLikelyBot);
