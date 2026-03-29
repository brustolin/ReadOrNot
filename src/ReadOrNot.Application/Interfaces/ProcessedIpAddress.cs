namespace ReadOrNot.Application.Interfaces;

public sealed record ProcessedIpAddress(string? StoredIpAddress, string? StoredIpHash, string? VisitorFingerprintHash);
