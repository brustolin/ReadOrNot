using System.ComponentModel.DataAnnotations;

namespace ReadOrNot.Application.DTOs.Account;

public sealed class UpdateAccountProfileRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? TimeZone { get; set; }
}
