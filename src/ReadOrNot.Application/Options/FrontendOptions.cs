namespace ReadOrNot.Application.Options;

public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";

    public string BaseUrl { get; set; } = "https://localhost:7067";
}
