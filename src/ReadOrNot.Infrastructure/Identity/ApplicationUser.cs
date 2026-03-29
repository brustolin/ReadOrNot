using Microsoft.AspNetCore.Identity;

namespace ReadOrNot.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public string? TimeZone { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
