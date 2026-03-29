namespace ReadOrNot.Application.Common;

public sealed class ConflictException(string message) : AppException(message);
