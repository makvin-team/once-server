using Microsoft.EntityFrameworkCore;
using Once.Application.Services.FraudAttempts.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Entities;
using Once.Domain.Enums;
using Once.Infrastructure.Persistence;
using System.Text.Json;

namespace Once.Application.Services.FraudAttempts;

public class FraudAttemptService(AppDbContext dbContext) : IFraudAttemptService
{
    public async Task<Result<List<FraudAttemptResponse>>> GetAllByUserAsync(long userId, CancellationToken ct = default)
    {
        var attempts = await dbContext.FraudAttempts
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.UserId == userId)
            .Include(a => a.Scenario)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

        return attempts.Select(a => MapToResponse(a)).ToList();
    }

    public async Task<Result<FraudStatsResponse>> GetStatsByUserAsync(long userId, CancellationToken ct = default)
    {
        var attempts = await dbContext.FraudAttempts
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.UserId == userId)
            .Select(a => new { a.Score, a.Passed })
            .ToListAsync(ct);

        if (attempts.Count == 0)
            return new FraudStatsResponse();

        return new FraudStatsResponse
        {
            TotalAttempts = attempts.Count,
            AverageScore  = (int)attempts.Average(a => a.Score),
            BestScore     = attempts.Max(a => a.Score),
            PassedCount   = attempts.Count(a => a.Passed),
            FailedCount   = attempts.Count(a => !a.Passed),
        };
    }

    public async Task<Result<FraudAttemptResponse>> SubmitAsync(
        long userId,
        SubmitFraudAttemptRequest request,
        CancellationToken ct = default)
    {
        var scenarioExists = await dbContext.FraudScenarios
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.ScenarioId && !s.IsDeleted, ct);

        if (!scenarioExists)
            return FraudAttemptErrors.ScenarioInvalid;

        var attempt = new FraudAttempt
        {
            ScenarioId         = request.ScenarioId,
            UserId             = userId,
            Score              = request.Score,
            Passed             = request.Passed,
            DetectedFlags      = request.DetectedFlags,
            MissedFlags        = request.MissedFlags,
            Status             = request.Passed ? FraudSimStatus.Completed : FraudSimStatus.Failed,
            SelectedDecisionId = request.SelectedDecisionId,
            SelectedFlagIds    = JsonSerializer.Serialize(request.SelectedFlagIds),
        };

        dbContext.FraudAttempts.Add(attempt);
        await dbContext.SaveChangesAsync(ct);

        var scenario = await dbContext.FraudScenarios
            .AsNoTracking()
            .Where(s => s.Id == request.ScenarioId)
            .Select(s => new { s.Title, s.FraudType })
            .SingleAsync(ct);

        return new FraudAttemptResponse
        {
            Id            = attempt.Id,
            ScenarioId    = attempt.ScenarioId,
            ScenarioTitle = scenario.Title,
            FraudType     = FraudTypeToString(scenario.FraudType),
            Score         = attempt.Score,
            Passed        = attempt.Passed,
            DetectedFlags = attempt.DetectedFlags,
            MissedFlags   = attempt.MissedFlags,
            AttemptedAt   = attempt.CreatedAt.ToString("yyyy-MM-dd"),
        };
    }

    private static FraudAttemptResponse MapToResponse(FraudAttempt a) => new()
    {
        Id            = a.Id,
        ScenarioId    = a.ScenarioId,
        ScenarioTitle = a.Scenario?.Title ?? "",
        FraudType     = FraudTypeToString(a.Scenario?.FraudType ?? FraudSimType.Phishing),
        Score         = a.Score,
        Passed        = a.Passed,
        DetectedFlags = a.DetectedFlags,
        MissedFlags   = a.MissedFlags,
        AttemptedAt   = a.CreatedAt.ToString("yyyy-MM-dd"),
    };

    private static string FraudTypeToString(FraudSimType t) => t switch
    {
        FraudSimType.Phishing          => "phishing",
        FraudSimType.Transaction       => "transaction",
        FraudSimType.Document          => "document",
        FraudSimType.DeepfakeCall      => "deepfake_call",
        FraudSimType.SocialEngineering => "social_engineering",
        FraudSimType.AmlKyc            => "aml_kyc",
        _                              => t.ToString().ToLower(),
    };
}
