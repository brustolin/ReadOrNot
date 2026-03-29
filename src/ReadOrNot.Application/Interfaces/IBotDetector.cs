namespace ReadOrNot.Application.Interfaces;

public interface IBotDetector
{
    BotDetectionResult Analyze(string? userAgent, string? acceptLanguage, string? queryString);
}
