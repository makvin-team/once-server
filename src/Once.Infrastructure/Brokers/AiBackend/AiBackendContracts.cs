using System.Text.Json;

namespace Once.Infrastructure.Brokers.AiBackend;

// ----- Outbound -----

/// <summary>Body sent to ai-backend POST /chat/stream.</summary>
public sealed record AiChatStreamRequest
{
    public Guid?   ConversationId         { get; init; }
    public string  Message                { get; init; } = string.Empty;
    public string  ExternalUserId         { get; init; } = string.Empty;
    /// <summary>Appended to ai-backend's system prompt for this turn.</summary>
    public string? AdditionalInstructions { get; init; }
}

// ----- Inbound -----

/// <summary>ai-backend PaginatedItems[T].</summary>
public sealed record AiPaged<T>
{
    public List<T> Items { get; init; } = [];
    public int     Total { get; init; }
    public int     Page  { get; init; }
    public int     Size  { get; init; }
    public int     Pages { get; init; }
}

/// <summary>ai-backend ConversationResponse.</summary>
public sealed record AiConversationDto
{
    public Guid      Id        { get; init; }
    public string?   Title     { get; init; }
    public DateTime  CreatedAt { get; init; }
    public DateTime  UpdatedAt { get; init; }
}

/// <summary>ai-backend MessageResponse (one row = one question/answer pair).</summary>
public sealed record AiMessageDto
{
    public Guid        Id                 { get; init; }
    public Guid        ConversationId     { get; init; }
    public string      Question           { get; init; } = string.Empty;
    public string      Answer             { get; init; } = string.Empty;
    public string?     Sources            { get; init; }
    public JsonElement? RegulatorySources { get; init; }
    public DateTime    CreatedAt          { get; init; }
}

/// <summary>ai-backend DocumentResponse (subset the knowledge page needs).</summary>
public sealed record AiDocumentDto
{
    public Guid     Id              { get; init; }
    public string   Filename        { get; init; } = string.Empty;
    public string?  ContentType     { get; init; }
    public long     Size            { get; init; }
    public string   EmbeddingStatus { get; init; } = string.Empty;
    public int?     ChunksCount     { get; init; }
    public string?  CollectionName  { get; init; }
    public DateTime CreatedAt       { get; init; }
}

/// <summary>ai-backend DocumentStatsResponse.</summary>
public sealed record AiDocumentStatsDto
{
    public int Total      { get; init; }
    public int Pending    { get; init; }
    public int Processing { get; init; }
    public int Embedded   { get; init; }
    public int Failed     { get; init; }
}
