using ReadOrNot.Application.DTOs.Account;
using ReadOrNot.Application.DTOs.Auth;

namespace ReadOrNot.Application.Interfaces;

public interface IAccountService
{
    Task<AccountProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken);

    Task<AccountProfileResponse> UpdateProfileAsync(Guid userId, UpdateAccountProfileRequest request, CancellationToken cancellationToken);

    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken);
}
