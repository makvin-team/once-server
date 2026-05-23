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
}
