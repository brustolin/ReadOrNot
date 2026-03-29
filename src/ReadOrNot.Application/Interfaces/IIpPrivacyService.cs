namespace ReadOrNot.Application.Interfaces;

public interface IIpPrivacyService
{
    ProcessedIpAddress Process(string? ipAddress, string? userAgent, string? acceptLanguage);
}
