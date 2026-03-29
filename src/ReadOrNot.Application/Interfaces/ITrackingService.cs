using ReadOrNot.Application.DTOs.Reports;

namespace ReadOrNot.Application.Interfaces;

public interface ITrackingService
{
    Task<TrackingHitResult> TrackAsync(TrackingRequestContext request, CancellationToken cancellationToken);
}
