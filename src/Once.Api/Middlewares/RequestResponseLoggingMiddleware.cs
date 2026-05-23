using System.Text;

namespace Once.Api.Middlewares;

/// <summary>
/// Global middleware for logging HTTP requests and responses
/// </summary>
public class RequestResponseLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestResponseLoggingMiddleware> logger)
{

    /// <summary>
    /// Invoke the middleware to log request and response
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        // =========================
        // REQUEST
        // =========================
        context.Request.EnableBuffering();

        string requestBody = await ReadRequestBody(context.Request);

        logger.LogInformation(
            "HTTP REQUEST {Method} {Path} Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            requestBody
        );

        // =========================
        // RESPONSE
        // =========================
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next(context);

        string responseText = await ReadResponseBody(responseBody);

        logger.LogInformation(
            "HTTP RESPONSE {StatusCode} {Path} Body: {Body}",
            context.Response.StatusCode,
            context.Request.Path,
            responseText
        );

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private static async Task<string> ReadResponseBody(Stream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        return text;
    }
}
