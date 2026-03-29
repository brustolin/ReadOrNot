namespace ReadOrNot.Application.Common;

public sealed class ValidationAppException(string message, IReadOnlyDictionary<string, string[]> errors) : AppException(message)
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
