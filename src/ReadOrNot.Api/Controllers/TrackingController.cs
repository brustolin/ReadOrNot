using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ReadOrNot.Application.DTOs.Reports;
using ReadOrNot.Application.Interfaces;

namespace ReadOrNot.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("t")]
[EnableRateLimiting("tracking")]
public sealed class TrackingController(
    ITrackingService trackingService,
    ITrackingAssetService trackingAssetService) : ControllerBase
{
    [HttpGet("{publicIdentifier}")]
    public async Task<IActionResult> Track(string publicIdentifier, CancellationToken cancellationToken)
    {
        await trackingService.TrackAsync(
            new TrackingRequestContext(
                publicIdentifier,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                Request.Headers.Referer.ToString(),
                Request.Headers.AcceptLanguage.ToString(),
                Request.QueryString.HasValue ? Request.QueryString.Value : null,
                Request.Method),
            cancellationToken);

        var asset = await trackingAssetService.GetDefaultAssetAsync(cancellationToken);

        Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, proxy-revalidate, max-age=0";
        Response.Headers.Pragma = "no-cache";
        Response.Headers.Expires = "0";

        return File(asset.Content, asset.ContentType, enableRangeProcessing: false);
    }
}
