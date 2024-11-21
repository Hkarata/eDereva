using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace eDereva.Api.Exceptions;

public class ProblemException(string error, string message) : Exception(message)
{
    public string Error { get; } = error;

    public new string Message { get; } = message;
}

public class ProblemExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ProblemException problemException) return true;

        var problemDetails = new ProblemDetails
        {
            Title = problemException.Error,
            Status = StatusCodes.Status400BadRequest,
            Detail = problemException.Message,
            Type = $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}"
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                ProblemDetails = problemDetails,
                HttpContext = httpContext
            }
        );
    }
}