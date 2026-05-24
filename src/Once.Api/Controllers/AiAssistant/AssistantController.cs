using Microsoft.AspNetCore.Mvc;
using Once.Api.Extensions;
using Once.Application.Services.AiAssistant;
using Once.Application.Services.AiAssistant.Contracts;

namespace Once.Api.Controllers.AiAssistant;

/// <summary>Learner AI assistant — proxies chat + history to ai-backend.</summary>
[Route("api/assistant")]
public class AssistantController(IAiAssistantService assistantService) : AuthorizedController
{
    /// <summary>Streams a chat answer as Server-Sent Events.</summary>
    [HttpPost("chat/stream")]
    public async Task ChatStreamAsync([FromBody] AiChatRequest request, CancellationToken ct)
    {
        Response.Headers.ContentType  = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no"; // disable nginx buffering

        await foreach (var frame in assistantService.StreamChatAsync(UserId, request, ct))
        {
            await Response.WriteAsync(frame, ct);
            await Response.Body.FlushAsync(ct);
        }
    }

    /// <summary>Lists the current user's conversations.</summary>
    [HttpGet("conversations")]
    public async Task<IResult> GetConversationsAsync(CancellationToken ct)
    {
        var result = await assistantService.GetConversationsAsync(UserId, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Returns the message history for one conversation (ownership-guarded).</summary>
    [HttpGet("conversations/{id:guid}/messages")]
    public async Task<IResult> GetMessagesAsync(Guid id, CancellationToken ct)
    {
        var result = await assistantService.GetMessagesAsync(UserId, id, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Deletes a conversation (ownership-guarded).</summary>
    [HttpDelete("conversations/{id:guid}")]
    public async Task<IResult> DeleteConversationAsync(Guid id, CancellationToken ct)
    {
        var result = await assistantService.DeleteConversationAsync(UserId, id, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }

    // ---- Citation document viewer ----
    // Proxy the cited document (regulatory act or uploaded file) from ai-backend
    // so the learner can open it to the exact article / page. Shared reference
    // material — authenticated, but not ownership-scoped.

    /// <summary>Regulatory document metadata for a cited source.</summary>
    [HttpGet("regulatory/documents/{id:guid}")]
    public Task GetRegulatoryDocumentAsync(Guid id, CancellationToken ct)
        => ProxyAsync(() => assistantService.GetRegulatoryDocumentAsync(id, ct), ct);

    /// <summary>Sibling language variants of a cited regulatory document.</summary>
    [HttpGet("regulatory/documents/{id:guid}/siblings")]
    public Task GetRegulatoryDocumentSiblingsAsync(Guid id, CancellationToken ct)
        => ProxyAsync(() => assistantService.GetRegulatoryDocumentSiblingsAsync(id, ct), ct);

    /// <summary>Streams a cited regulatory document body (HTML inline / PDF).</summary>
    [HttpGet("regulatory/documents/{id:guid}/content")]
    public Task GetRegulatoryDocumentContentAsync(Guid id, CancellationToken ct)
        => ProxyAsync(() => assistantService.GetRegulatoryDocumentContentAsync(id, ct), ct);

    /// <summary>Streams a cited uploaded knowledge-base file body (e.g. a PDF).</summary>
    [HttpGet("documents/content/{filename}")]
    public Task GetKnowledgeFileContentAsync(string filename, CancellationToken ct)
        => ProxyAsync(() => assistantService.GetKnowledgeFileContentAsync(filename, ct), ct);

    // Copies the upstream status, content-type, and content-disposition, then
    // streams the body straight to the client. A 5xx with no body becomes a
    // clean 502 so the SPA can show a "couldn't load document" state.
    private async Task ProxyAsync(Func<Task<HttpResponseMessage>> send, CancellationToken ct)
    {
        HttpResponseMessage upstream;
        try
        {
            upstream = await send();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            Response.StatusCode = StatusCodes.Status502BadGateway;
            return;
        }

        using (upstream)
        {
            Response.StatusCode = (int)upstream.StatusCode;
            if (upstream.Content.Headers.ContentType is { } contentType)
                Response.Headers.ContentType = contentType.ToString();
            if (upstream.Content.Headers.TryGetValues("Content-Disposition", out var disposition))
                Response.Headers.ContentDisposition = disposition.ToArray();

            await using var stream = await upstream.Content.ReadAsStreamAsync(ct);
            await stream.CopyToAsync(Response.Body, ct);
        }
    }
}
