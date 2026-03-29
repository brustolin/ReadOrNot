using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadOrNot.Api.Extensions;
using ReadOrNot.Application.DTOs.Reports;
using ReadOrNot.Application.DTOs.Tokens;
using ReadOrNot.Application.Interfaces;

namespace ReadOrNot.Api.Controllers;

[ApiController]
[Authorize]
[Route("tokens")]
public sealed class TokensController(ITokenService tokenService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<TrackingTokenListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TrackingTokenListItemDto>>> GetTokens(CancellationToken cancellationToken)
    {
        return Ok(await tokenService.GetTokensAsync(User.GetRequiredUserId(), cancellationToken));
    }

    [HttpGet("{tokenId:int}")]
    [ProducesResponseType<TrackingTokenDetailsDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TrackingTokenDetailsDto>> GetToken(
        int tokenId,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] bool includeLikelyBots = true,
        CancellationToken cancellationToken = default)
    {
        var query = new TokenReportQuery
        {
            FromUtc = fromUtc,
            ToUtc = toUtc,
            IncludeLikelyBots = includeLikelyBots
        };

        return Ok(await tokenService.GetTokenAsync(User.GetRequiredUserId(), tokenId, query, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<TrackingTokenDetailsDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<TrackingTokenDetailsDto>> CreateToken(CreateTrackingTokenRequest request, CancellationToken cancellationToken)
    {
        var token = await tokenService.CreateTokenAsync(User.GetRequiredUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetToken), new { tokenId = token.Id }, token);
    }

    [HttpPut("{tokenId:int}")]
    [ProducesResponseType<TrackingTokenDetailsDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TrackingTokenDetailsDto>> UpdateToken(int tokenId, UpdateTrackingTokenRequest request, CancellationToken cancellationToken)
    {
        return Ok(await tokenService.UpdateTokenAsync(User.GetRequiredUserId(), tokenId, request, cancellationToken));
    }

    [HttpPost("{tokenId:int}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EnableToken(int tokenId, CancellationToken cancellationToken)
    {
        await tokenService.SetTokenEnabledAsync(User.GetRequiredUserId(), tokenId, true, cancellationToken);
        return NoContent();
    }

    [HttpPost("{tokenId:int}/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DisableToken(int tokenId, CancellationToken cancellationToken)
    {
        await tokenService.SetTokenEnabledAsync(User.GetRequiredUserId(), tokenId, false, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{tokenId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteToken(int tokenId, CancellationToken cancellationToken)
    {
        await tokenService.DeleteTokenAsync(User.GetRequiredUserId(), tokenId, cancellationToken);
        return NoContent();
    }
}
