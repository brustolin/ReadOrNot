using ReadOrNot.Application.Interfaces;

namespace ReadOrNot.Infrastructure.BotDetection;

internal sealed class HeuristicBotDetector : IBotDetector
{
    private static readonly string[] BotMarkers =
    [
        "bot",
        "spider",
        "crawler",
        "preview",
        "prefetch",
        "headless",
        "scanner",
        "virus",
        "linkexpanding",
        "facebookexternalhit",
        "slackbot",
        "whatsapp",
        "discordbot",
        "googleimageproxy",
        "microsoft office",
        "outlook-ios",
        "proofpoint",
        "mimecast"
    ];

    public BotDetectionResult Analyze(string? userAgent, string? acceptLanguage, string? queryString)
    {
        var signals = new List<string>();
        var normalizedUserAgent = userAgent?.Trim().ToLowerInvariant() ?? string.Empty;
        var normalizedQueryString = queryString?.Trim().ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedUserAgent))
        {
            signals.Add("missing-user-agent");
        }

        if (string.IsNullOrWhiteSpace(acceptLanguage))
        {
            signals.Add("missing-accept-language");
        }

        foreach (var marker in BotMarkers.Where(normalizedUserAgent.Contains))
        {
            signals.Add($"ua:{marker}");
        }

        if (normalizedQueryString.Contains("prefetch", StringComparison.Ordinal))
        {
            signals.Add("query:prefetch");
        }

        return new BotDetectionResult(signals.Count > 0, signals.Count == 0 ? null : string.Join(", ", signals.Distinct()));
    }
}
