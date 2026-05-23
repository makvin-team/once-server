using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Once.Api.Filters;

public class ModelValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid)
        {
            await next();
            return;
        }

        var firstError = context.ModelState
            .Where(x => x.Value != null && x.Value.Errors.Count > 0)
            .Select(x => x.Value!.Errors.First().ErrorMessage)
            .FirstOrDefault();

        var errorObj = TryParseErrorMessage(firstError ?? string.Empty);

            var problemDetails = new ProblemDetails
        {
            Title = "Validation Error",
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        problemDetails.Extensions["errors"] = errorObj;

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }

    private object TryParseErrorMessage(string errorMessage)
    {
        try
        {
            return JsonSerializer.Deserialize<object>(errorMessage)
                ?? new { Code = "Validation", Description = errorMessage, Type = "Validation" };
        }
        catch
        {
            return new
            {
                Code = "Validation",
                Description = errorMessage,
                Type = "Validation"
            };
        }
    }
}
