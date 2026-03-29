namespace ReadOrNot.Application.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "SqlServer";

    public string ConnectionStringName { get; set; } = "SqlServer";

    public string MySqlServerVersion { get; set; } = "8.0.36";
}
