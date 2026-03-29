namespace ReadOrNot.Domain.Entities;

public sealed class TrackingToken(Guid userId, string publicIdentifier, string name, string? description, DateTime createdAtUtc, DateTime? expiresAtUtc)
{
    public int Id { get; private set; }

    public Guid UserId { get; private set; } = userId;

    public string PublicIdentifier { get; private set; } = publicIdentifier;

    public string Name { get; private set; } = name;

    public string? Description { get; private set; } = description;

    public DateTime CreatedAtUtc { get; private set; } = createdAtUtc;

    public DateTime? ExpiresAtUtc { get; private set; } = expiresAtUtc;

    public bool IsEnabled { get; private set; } = true;

    public ICollection<OpenEvent> OpenEvents { get; private set; } = [];

    public void UpdateDetails(string name, string? description, DateTime? expiresAtUtc)
    {
        Name = name;
        Description = description;
        ExpiresAtUtc = expiresAtUtc;
    }

    public void SetEnabled(bool enabled)
    {
        IsEnabled = enabled;
    }

    public bool IsExpired(DateTime utcNow)
    {
        return ExpiresAtUtc.HasValue && ExpiresAtUtc.Value <= utcNow;
    }

    public bool CanTrack(DateTime utcNow)
    {
        return IsEnabled && !IsExpired(utcNow);
    }
}
