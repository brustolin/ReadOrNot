using Microsoft.Extensions.Options;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Application.Options;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class TokenPublicUrlBuilder(IOptions<TrackingOptions> trackingOptions) : ITokenPublicUrlBuilder
{
    private readonly TrackingOptions _trackingOptions = trackingOptions.Value;

    public string BuildTrackingUrl(string publicIdentifier)
    {
        var baseUrl = _trackingOptions.PublicBaseUrl.TrimEnd('/');
        var prefix = _trackingOptions.EndpointPrefix.Trim('/').Trim();
        return $"{baseUrl}/{prefix}/{publicIdentifier}";
    }
}
