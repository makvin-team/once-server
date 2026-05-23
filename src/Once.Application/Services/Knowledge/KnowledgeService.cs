using System.Net;
using Once.Domain.Abstractions;
using Once.Infrastructure.Brokers.AiBackend;

namespace Once.Application.Services.Knowledge;

public sealed class KnowledgeService(IAiBackendBroker broker) : IKnowledgeService
{
    public async Task<Result<AiDocumentDto>> UploadAsync(
        Stream content, string fileName, string contentType, CancellationToken ct)
    {
        try
        {
            return await broker.UploadDocumentAsync(content, fileName, contentType, ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            return KnowledgeErrors.UpstreamForbidden;
        }
        catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            return KnowledgeErrors.Upstream;
        }
    }

    public async Task<Result<AiPaged<AiDocumentDto>>> GetDocumentsAsync(
        string? status, int limit, CancellationToken ct)
    {
        try
        {
            return await broker.GetDocumentsAsync(status, limit, ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            return KnowledgeErrors.UpstreamForbidden;
        }
        catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            return KnowledgeErrors.Upstream;
        }
    }

    public async Task<Result<AiDocumentStatsDto>> GetStatsAsync(CancellationToken ct)
    {
        try
        {
            return await broker.GetDocumentStatsAsync(ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            return KnowledgeErrors.UpstreamForbidden;
        }
        catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            return KnowledgeErrors.Upstream;
        }
    }
}
