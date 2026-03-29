namespace ReadOrNot.Application.Options;

public sealed class TrackingOptions
{
    public const string SectionName = "Tracking";

    public string PublicBaseUrl { get; set; } = "https://localhost:7040";

    public string AssetPath { get; set; } = "Assets/tracking-pixel.svg";

    public string AssetContentType { get; set; } = "image/svg+xml";

    public string EndpointPrefix { get; set; } = "t";
}
