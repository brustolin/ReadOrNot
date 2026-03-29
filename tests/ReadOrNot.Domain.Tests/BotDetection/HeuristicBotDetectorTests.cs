using ReadOrNot.Infrastructure.BotDetection;

namespace ReadOrNot.Domain.Tests.BotDetection;

public sealed class HeuristicBotDetectorTests
{
    [Fact]
    public void Analyze_FlagsKnownBotMarkers()
    {
        var detector = new HeuristicBotDetector();

        var result = detector.Analyze("GoogleImageProxy", "en-US", null);

        Assert.True(result.IsLikelyBot);
        Assert.Contains("ua:googleimageproxy", result.Signals);
    }

    [Fact]
    public void Analyze_FlagsMissingAcceptLanguage()
    {
        var detector = new HeuristicBotDetector();

        var result = detector.Analyze("Mozilla/5.0", null, null);

        Assert.True(result.IsLikelyBot);
        Assert.Contains("missing-accept-language", result.Signals);
    }
}
