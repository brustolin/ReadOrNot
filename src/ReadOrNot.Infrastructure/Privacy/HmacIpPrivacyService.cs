using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Application.Options;
using ReadOrNot.Domain.Enums;

namespace ReadOrNot.Infrastructure.Privacy;

internal sealed class HmacIpPrivacyService(
    IOptions<PrivacyOptions> privacyOptions,
    ILogger<HmacIpPrivacyService> logger) : IIpPrivacyService
{
    private readonly PrivacyOptions _privacyOptions = privacyOptions.Value;

    public ProcessedIpAddress Process(string? ipAddress, string? userAgent, string? acceptLanguage)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return new ProcessedIpAddress(null, null, HashFingerprint("missing-ip", userAgent, acceptLanguage));
        }

        return _privacyOptions.IpStorageMode switch
        {
            IpStorageMode.Full => new ProcessedIpAddress(ipAddress, HashWithKey(ipAddress), HashFingerprint(ipAddress, userAgent, acceptLanguage)),
            IpStorageMode.Hashed => new ProcessedIpAddress(null, HashWithKey(ipAddress), HashFingerprint(HashWithKey(ipAddress), userAgent, acceptLanguage)),
            IpStorageMode.None => new ProcessedIpAddress(null, null, HashFingerprint(null, userAgent, acceptLanguage)),
            _ => throw new InvalidOperationException($"Unsupported IP storage mode '{_privacyOptions.IpStorageMode}'.")
        };
    }

    private string HashWithKey(string value)
    {
        var key = _privacyOptions.IpHashKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            logger.LogWarning("Privacy:IpHashKey is not configured. Falling back to an unkeyed SHA256 hash for development use.");
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    private string HashFingerprint(string? ipComponent, string? userAgent, string? acceptLanguage)
    {
        var composite = $"{ipComponent}|{userAgent?.Trim()}|{acceptLanguage?.Trim()}";
        return HashWithKey(composite);
    }
}
