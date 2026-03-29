namespace ReadOrNot.Application.Options;

public sealed class RetentionOptions
{
    public const string SectionName = "Retention";

    public bool PurgeEnabled { get; set; } = false;

    public int OpenEventsRetentionDays { get; set; } = 90;

    public int CleanupIntervalHours { get; set; } = 24;
}
