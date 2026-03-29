using System.ComponentModel.DataAnnotations;

namespace ReadOrNot.Application.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = "ReadOrNot";

    [Required]
    public string Audience { get; set; } = "ReadOrNot.Client";

    [Required]
    [MinLength(32)]
    public string SigningKey { get; set; } = "change-this-development-signing-key-please";

    [Range(5, 1440)]
    public int AccessTokenLifetimeMinutes { get; set; } = 60;
}
