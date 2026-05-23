using Once.Domain.Abstractions;

namespace Once.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(
        this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }
        
        return Results.Problem(
            statusCode: GetStatusCode(result.Error.Type),
            title: GetTitle(result.Error.Type),
            extensions: new Dictionary<string, object?>
            {
                { "errors", new
                    {
                        result.Error.Code,
                        Description = result.Error.Code,
                        result.Error.Type
                    }
                }
            },
            type: GetType(result.Error.Type));

        static int GetStatusCode(ErrorType type) =>
            type switch
            {
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Processing => StatusCodes.Status202Accepted,
                _ => StatusCodes.Status500InternalServerError
            };
        
        static string GetTitle(ErrorType type) =>
            type switch
            {
                ErrorType.Failure => "Internal Server Error",
                ErrorType.Validation => "Bad Request",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                ErrorType.Processing => "Processing",
                _ => "Internal Server Error"
            };
        
        static string GetType(ErrorType type) =>
            type switch
            {
                ErrorType.Failure => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                ErrorType.Processing => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.3",
                _ => "Internal Server Error"
            };
    }
}