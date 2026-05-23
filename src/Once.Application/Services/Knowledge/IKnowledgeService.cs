using Once.Domain.Abstractions;
using Once.Infrastructure.Brokers.AiBackend;

namespace Once.Application.Services.Knowledge;

public interface IKnowledgeService
{
    Task<Result<AiDocumentDto>> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct);

    Task<Result<AiPaged<AiDocumentDto>>> GetDocumentsAsync(string? status, int limit, CancellationToken ct);

    Task<Result<AiDocumentStatsDto>> GetStatsAsync(CancellationToken ct);
}
