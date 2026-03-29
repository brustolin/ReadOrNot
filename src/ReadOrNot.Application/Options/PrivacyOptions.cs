using ReadOrNot.Domain.Enums;

namespace ReadOrNot.Application.Options;

public sealed class PrivacyOptions
{
    public const string SectionName = "Privacy";

    public IpStorageMode IpStorageMode { get; set; } = IpStorageMode.Hashed;

    public string? IpHashKey { get; set; }
}
