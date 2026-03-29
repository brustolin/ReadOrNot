using ReadOrNot.Application.Interfaces;

namespace ReadOrNot.Infrastructure.Time;

internal sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
