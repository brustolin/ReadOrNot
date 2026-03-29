namespace ReadOrNot.Application.Interfaces;

public sealed record TrackingAssetResult(byte[] Content, string ContentType, string FileName);
