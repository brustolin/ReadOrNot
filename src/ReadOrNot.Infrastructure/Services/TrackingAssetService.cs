using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Application.Options;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class TrackingAssetService(
    IHostEnvironment hostEnvironment,
    IOptions<TrackingOptions> trackingOptions) : ITrackingAssetService
{
    private readonly TrackingOptions _trackingOptions = trackingOptions.Value;

    public async Task<TrackingAssetResult> GetDefaultAssetAsync(CancellationToken cancellationToken)
    {
        var assetPath = Path.Combine(hostEnvironment.ContentRootPath, _trackingOptions.AssetPath);
        var content = await File.ReadAllBytesAsync(assetPath, cancellationToken);
        return new TrackingAssetResult(content, _trackingOptions.AssetContentType, Path.GetFileName(assetPath));
    }
}
