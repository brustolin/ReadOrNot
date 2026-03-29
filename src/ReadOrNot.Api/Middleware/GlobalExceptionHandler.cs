using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReadOrNot.Application.Common;

namespace ReadOrNot.Api.Middleware;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}.", httpContext.Request.Method, httpContext.Request.Path);

        var problemDetails = exception switch
        {
            ValidationAppException validationException => new HttpValidationProblemDetails(validationException.Errors)
            {
                Title = validationException.Message,
                Status = StatusCodes.Status400BadRequest
            },
            NotFoundException notFoundException => new ProblemDetails
            {
                Title = notFoundException.Message,
                Status = StatusCodes.Status404NotFound
            },
            ConflictException conflictException => new ProblemDetails
            {
                Title = conflictException.Message,
                Status = StatusCodes.Status409Conflict
            },
            _ => new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
