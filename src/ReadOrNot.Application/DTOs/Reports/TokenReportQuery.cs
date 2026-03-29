namespace ReadOrNot.Application.DTOs.Reports;

public sealed class TokenReportQuery
{
    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    public bool IncludeLikelyBots { get; set; } = true;
}
