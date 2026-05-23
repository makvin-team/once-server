using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Once.Application.Services.AiAssistant.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Entities;
using Once.Infrastructure.Brokers.AiBackend;
using Once.Infrastructure.Persistence;

namespace Once.Application.Services.AiAssistant;

public sealed class AiAssistantService(
    AppDbContext dbContext,
    IAiBackendBroker broker) : IAiAssistantService
{
    public async IAsyncEnumerable<string> StreamChatAsync(
        long userId, AiChatRequest request, [EnumeratorCancellation] CancellationToken ct)
    {
        // Existing conversation → must be owned by this user.
        if (request.ConversationId is Guid existing)
        {
            var owns = await dbContext.AiConversations
                .AsNoTracking()
                .AnyAsync(c => c.ConversationId == existing
                            && c.OwnerUserId == userId
                            && !c.IsDeleted, ct);
            if (!owns)
            {
                yield return SseError("Conversation not found");
                yield break;
            }
        }

        var upstream = new AiChatStreamRequest
        {
            ConversationId = request.ConversationId,
            Message        = request.Message,
            ExternalUserId = userId.ToString(),
        };

        HttpResponseMessage? response = null;
        try
        {
            response = await broker.OpenChatStreamAsync(upstream, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            // Swallow here; we surface a clean SSE error frame below (yield is not
            // permitted inside a catch block).
        }

        if (response is null || !response.IsSuccessStatusCode)
        {
            response?.Dispose();
            yield return SseError("Upstream error");
            yield break;
        }

        using (response)
        {
            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync(ct)) is not null)
            {
                // Reconstruct SSE framing: ai-backend emits `data: {json}` then a blank line.
                yield return line.Length == 0 ? "\n" : line + "\n";

                // Only the `done` frame carries a conversation_id to persist; the cheap
                // substring guard avoids parsing JSON for every streamed chunk token.
                if (line.StartsWith("data:", StringComparison.Ordinal)
                    && line.Contains("\"done\"", StringComparison.Ordinal))
                    await TryRecordConversationAsync(userId, line["data:".Length..].Trim(), ct);
            }
        }
    }

    public async Task<Result<AiPaged<AiConversationDto>>> GetConversationsAsync(
        long userId, CancellationToken ct)
    {
        try
        {
            var page = await broker.GetConversationsAsync(userId.ToString(), ct);

            // Self-heal the local map for any conversation ai-backend returns for this
            // user. Load known ids in one query, then add only the missing ones. (No
            // !IsDeleted filter: the unique index on ConversationId means at most one
            // row per conversation, so this prevents a duplicate-insert constraint hit.)
            var ids = page.Items.Select(c => c.Id).ToList();
            var knownIds = await dbContext.AiConversations
                .Where(x => ids.Contains(x.ConversationId))
                .Select(x => x.ConversationId)
                .ToListAsync(ct);
            var knownSet = knownIds.ToHashSet();
            foreach (var c in page.Items.Where(c => !knownSet.Contains(c.Id)))
            {
                dbContext.AiConversations.Add(new AiConversation
                {
                    ConversationId = c.Id,
                    OwnerUserId    = userId,
                });
            }
            await dbContext.SaveChangesAsync(ct);

            return Result<AiPaged<AiConversationDto>>.Success(page);
        }
        catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            return AiAssistantErrors.Upstream;
        }
    }

    public async Task<Result<AiPaged<AiMessageDto>>> GetMessagesAsync(
        long userId, Guid conversationId, CancellationToken ct)
    {
        if (!await OwnsAsync(userId, conversationId, ct))
            return AiAssistantErrors.ConversationNotFound;

        try
        {
            return await broker.GetMessagesAsync(conversationId, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            return AiAssistantErrors.Upstream;
        }
    }

    public async Task<Result> DeleteConversationAsync(
        long userId, Guid conversationId, CancellationToken ct)
    {
        var row = await dbContext.AiConversations
            .SingleOrDefaultAsync(c => c.ConversationId == conversationId
                                    && c.OwnerUserId == userId
                                    && !c.IsDeleted, ct);
        if (row is null)
            return AiAssistantErrors.ConversationNotFound;

        try
        {
            await broker.DeleteConversationAsync(conversationId, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            return AiAssistantErrors.Upstream;
        }

        row.IsDeleted = true;
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<bool> OwnsAsync(long userId, Guid conversationId, CancellationToken ct) =>
        await dbContext.AiConversations
            .AsNoTracking()
            .AnyAsync(c => c.ConversationId == conversationId
                        && c.OwnerUserId == userId
                        && !c.IsDeleted, ct);

    private async Task TryRecordConversationAsync(long userId, string json, CancellationToken ct)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return;
            if (!root.TryGetProperty("type", out var type) || type.GetString() != "done") return;
            if (!root.TryGetProperty("conversation_id", out var idEl)) return;
            if (!Guid.TryParse(idEl.GetString(), out var conversationId)) return;

            var exists = await dbContext.AiConversations
                .AnyAsync(x => x.ConversationId == conversationId, ct);
            if (exists) return;

            dbContext.AiConversations.Add(new AiConversation
            {
                ConversationId = conversationId,
                OwnerUserId    = userId,
            });
            await dbContext.SaveChangesAsync(ct);
        }
        catch (JsonException)
        {
            // Non-JSON or partial line — ignore; only the `done` frame matters here.
        }
    }

    private static string SseError(string message)
    {
        var json = JsonSerializer.Serialize(new { type = "error", message });
        return $"data: {json}\n\n";
    }
}
