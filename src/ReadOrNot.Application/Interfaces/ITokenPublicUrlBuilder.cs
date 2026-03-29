namespace ReadOrNot.Application.Interfaces;

public interface ITokenPublicUrlBuilder
{
    string BuildTrackingUrl(string publicIdentifier);
}
