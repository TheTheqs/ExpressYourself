using ExpressYourself.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExpressYourself.Api.ExceptionHandling;

/// <summary>
/// Handles unhandled exceptions globally and converts them into standardized HTTP responses.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle the exception and write a standardized error response.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The thrown exception.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>true</c> if the exception was handled; otherwise, <c>false</c>.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(httpContext, exception);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance based on the exception type.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The thrown exception.</param>
    /// <returns>A configured <see cref="ProblemDetails"/> instance.</returns>
    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        Exception exception)
    {
        return exception switch
        {
            ArgumentException argumentException => new ProblemDetails
            {
                Title = "Invalid request",
                Detail = argumentException.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Instance = httpContext.Request.Path
            },

            DomainException domainException => new ProblemDetails
            {
                Title = "Domain validation error",
                Detail = domainException.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Instance = httpContext.Request.Path
            },

            KeyNotFoundException keyNotFoundException => new ProblemDetails
            {
                Title = "Resource not found",
                Detail = keyNotFoundException.Message,
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Instance = httpContext.Request.Path
            },

            InvalidOperationException invalidOperationException => new ProblemDetails
            {
                Title = "Invalid operation",
                Detail = invalidOperationException.Message,
                Status = StatusCodes.Status409Conflict,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                Instance = httpContext.Request.Path
            },

            _ => new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while processing the request.",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Instance = httpContext.Request.Path
            }
        };
    }
}