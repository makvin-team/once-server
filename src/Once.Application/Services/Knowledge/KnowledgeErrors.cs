using Once.Domain.Abstractions;

namespace Once.Application.Services.Knowledge;

public static class KnowledgeErrors
{
    public static Error Upstream          => Error.Failure("Knowledge.Upstream");
    // 403 from ai-backend: the api-key user is not an admin, or knowledge_base
    // is not in its allowed collections. Distinct code so the client can explain.
    public static Error UpstreamForbidden => Error.Failure("Knowledge.UpstreamForbidden");
}
