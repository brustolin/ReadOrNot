using System.ComponentModel.DataAnnotations;

namespace ReadOrNot.Application.DTOs.Tokens;

public sealed class CreateTrackingTokenRequest
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }
}
