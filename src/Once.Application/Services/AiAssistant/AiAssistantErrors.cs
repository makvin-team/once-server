using Once.Domain.Abstractions;

namespace Once.Application.Services.AiAssistant;

public static class AiAssistantErrors
{
    // NotFound is intentional for ownership failures — does not reveal existence.
    public static Error ConversationNotFound => Error.NotFound("AiAssistant.ConversationNotFound");
    public static Error Upstream             => Error.Failure("AiAssistant.Upstream");
}
