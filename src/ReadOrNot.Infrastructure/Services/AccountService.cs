using Microsoft.AspNetCore.Identity;
using ReadOrNot.Application.Common;
using ReadOrNot.Application.DTOs.Account;
using ReadOrNot.Application.DTOs.Auth;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Infrastructure.Identity;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class AccountService(UserManager<ApplicationUser> userManager) : IAccountService
{
    public async Task<AccountProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId);
        return MapProfile(user);
    }

    public async Task<AccountProfileResponse> UpdateProfileAsync(Guid userId, UpdateAccountProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId);
        user.DisplayName = request.DisplayName.Trim();
        user.TimeZone = string.IsNullOrWhiteSpace(request.TimeZone) ? null : request.TimeZone.Trim();

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new ValidationAppException(
                "Profile update failed.",
                result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }, StringComparer.OrdinalIgnoreCase));
        }

        return MapProfile(user);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId);
        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new ValidationAppException(
                "Password change failed.",
                result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }, StringComparer.OrdinalIgnoreCase));
        }
    }

    private async Task<ApplicationUser> GetUserAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user ?? throw new NotFoundException("Account was not found.");
    }

    private static AccountProfileResponse MapProfile(ApplicationUser user)
    {
        return new AccountProfileResponse(user.Id, user.Email!, user.DisplayName, user.TimeZone, user.CreatedAtUtc);
    }
}
