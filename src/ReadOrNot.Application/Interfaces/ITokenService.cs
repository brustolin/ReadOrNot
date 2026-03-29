using ReadOrNot.Application.DTOs.Reports;
using ReadOrNot.Application.DTOs.Tokens;

namespace ReadOrNot.Application.Interfaces;

public interface ITokenService
{
    Task<IReadOnlyCollection<TrackingTokenListItemDto>> GetTokensAsync(Guid userId, CancellationToken cancellationToken);

    Task<TrackingTokenDetailsDto> GetTokenAsync(Guid userId, int tokenId, TokenReportQuery query, CancellationToken cancellationToken);

    Task<TrackingTokenDetailsDto> CreateTokenAsync(Guid userId, CreateTrackingTokenRequest request, CancellationToken cancellationToken);

    Task<TrackingTokenDetailsDto> UpdateTokenAsync(Guid userId, int tokenId, UpdateTrackingTokenRequest request, CancellationToken cancellationToken);

    Task SetTokenEnabledAsync(Guid userId, int tokenId, bool isEnabled, CancellationToken cancellationToken);

    Task DeleteTokenAsync(Guid userId, int tokenId, CancellationToken cancellationToken);
}
