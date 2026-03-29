using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using ReadOrNot.Application.Interfaces;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class TokenIdentifierGenerator : ITokenIdentifierGenerator
{
    public string Create()
    {
        Span<byte> buffer = stackalloc byte[24];
        RandomNumberGenerator.Fill(buffer);
        return WebEncoders.Base64UrlEncode(buffer);
    }
}
