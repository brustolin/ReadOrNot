using ReadOrNot.Domain.Entities;

namespace ReadOrNot.Domain.Tests.Tokens;

public sealed class TrackingTokenTests
{
    [Fact]
    public void CanTrack_ReturnsFalse_WhenTokenIsExpired()
    {
        var token = new TrackingToken(Guid.NewGuid(), "public-id", "Spring launch", null, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddMinutes(-1));

        var result = token.CanTrack(DateTime.UtcNow);

        Assert.False(result);
    }

    [Fact]
    public void CanTrack_ReturnsFalse_WhenTokenIsDisabled()
    {
        var token = new TrackingToken(Guid.NewGuid(), "public-id", "Newsletter", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(2));
        token.SetEnabled(false);

        var result = token.CanTrack(DateTime.UtcNow);

        Assert.False(result);
    }
}
