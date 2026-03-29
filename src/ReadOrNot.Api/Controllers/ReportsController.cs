using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadOrNot.Api.Extensions;
using ReadOrNot.Application.DTOs.Reports;
using ReadOrNot.Application.DTOs.Tokens;
using ReadOrNot.Application.Interfaces;

namespace ReadOrNot.Api.Controllers;

[ApiController]
[Authorize]
[Route("reports")]
public sealed class ReportsController(ITokenService tokenService) : ControllerBase
{
    [HttpGet("tokens/{tokenId:int}")]
    [ProducesResponseType<TrackingTokenDetailsDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TrackingTokenDetailsDto>> GetTokenReport(
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
}
