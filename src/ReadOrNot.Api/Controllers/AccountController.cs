using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadOrNot.Api.Extensions;
using ReadOrNot.Application.DTOs.Account;
using ReadOrNot.Application.DTOs.Auth;
using ReadOrNot.Application.Interfaces;

namespace ReadOrNot.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/account")]
public sealed class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<AccountProfileResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AccountProfileResponse>> GetProfile(CancellationToken cancellationToken)
    {
        return Ok(await accountService.GetProfileAsync(User.GetRequiredUserId(), cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType<AccountProfileResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AccountProfileResponse>> UpdateProfile(UpdateAccountProfileRequest request, CancellationToken cancellationToken)
    {
        return Ok(await accountService.UpdateProfileAsync(User.GetRequiredUserId(), request, cancellationToken));
    }

    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await accountService.ChangePasswordAsync(User.GetRequiredUserId(), request, cancellationToken);
        return NoContent();
    }
}
