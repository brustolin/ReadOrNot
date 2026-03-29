namespace ReadOrNot.Application.Interfaces;

public interface ITokenRetentionService
{
    Task<int> PurgeOldEventsAsync(CancellationToken cancellationToken);
}
