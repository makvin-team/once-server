using Once.Application.Helpers;
using Once.Domain.Abstractions;
using Once.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Once.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception is ApiException apiException
            ? apiException.StatusCode
            : (int)HttpStatusCode.InternalServerError;

        var error = Error.InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Title = EnvironmentHelper.IsStaging || EnvironmentHelper.IsDevelopment ? exception.Message : "Internal Server Error",
            Status = statusCode,
            Type = statusCode == (int)HttpStatusCode.Unauthorized
                ? "https://tools.ietf.org/html/rfc7235#section-3.1"
                : "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Extensions =
            {
                ["errors"] = new
                {
                    error.Code,
                    Description = error.Code,
                    error.Type
                }
            }
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
