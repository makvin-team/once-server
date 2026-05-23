using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Once.Api.Extensions;
using Once.Application.Services.Knowledge;

namespace Once.Api.Controllers.Knowledge;

/// <summary>Admin knowledge base — proxies document upload/list to ai-backend.</summary>
[Route("api/knowledge")]
[Authorize(Roles = "Admin")]
public class KnowledgeController(IKnowledgeService knowledgeService) : AuthorizedController
{
    /// <summary>Uploads one document into the knowledge_base collection.</summary>
    [HttpPost("documents")]
    public async Task<IResult> UploadAsync(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var result = await knowledgeService.UploadAsync(stream, file.FileName, file.ContentType, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Lists knowledge-base documents (optionally filtered by embedding status).</summary>
    [HttpGet("documents")]
    public async Task<IResult> GetDocumentsAsync(
        [FromQuery] string? status, [FromQuery] int limit = 100, CancellationToken ct = default)
    {
        var result = await knowledgeService.GetDocumentsAsync(status, limit, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Per-status document counts.</summary>
    [HttpGet("documents/stats")]
    public async Task<IResult> GetStatsAsync(CancellationToken ct)
    {
        var result = await knowledgeService.GetStatsAsync(ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }
}
