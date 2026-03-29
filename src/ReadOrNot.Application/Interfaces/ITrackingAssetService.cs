namespace ReadOrNot.Application.Interfaces;

public interface ITrackingAssetService
{
    Task<TrackingAssetResult> GetDefaultAssetAsync(CancellationToken cancellationToken);
}
