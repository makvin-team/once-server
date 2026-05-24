using Microsoft.EntityFrameworkCore;
using Once.Application.Services.FraudScenarios.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Enums;
using Once.Infrastructure.Persistence;

namespace Once.Application.Services.FraudScenarios;

public class FraudScenarioService(AppDbContext dbContext) : IFraudScenarioService
{
    public async Task<Result<List<FraudScenarioResponse>>> GetAllAsync(long? userId, CancellationToken ct = default)
    {
        var scenarios = await dbContext.FraudScenarios
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .ToListAsync(ct);

        var statMap = new Dictionary<long, (int Best, string Status)>();

        if (userId.HasValue)
        {
            var attempts = await dbContext.FraudAttempts
                .AsNoTracking()
                .Where(a => !a.IsDeleted && a.UserId == userId.Value)
                .Select(a => new { a.ScenarioId, a.Score, a.Passed })
                .ToListAsync(ct);

            statMap = attempts
                .GroupBy(a => a.ScenarioId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var best   = g.Max(a => a.Score);
                        var passed = g.Any(a => a.Passed);
                        return (best, passed ? "completed" : "failed");
                    });
        }

        var result = scenarios.Select(s =>
        {
            statMap.TryGetValue(s.Id, out var stat);
            return MapToResponse(s,
                stat.Best == 0 && !statMap.ContainsKey(s.Id) ? null : (int?)stat.Best,
                statMap.ContainsKey(s.Id) ? stat.Status : null);
        }).ToList();

        return result;
    }

    public async Task<Result<FraudScenarioResponse>> GetByIdAsync(long id, long? userId, CancellationToken ct = default)
    {
        var scenario = await dbContext.FraudScenarios
            .AsNoTracking()
            .Where(s => s.Id == id && !s.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (scenario is null)
            return FraudScenarioErrors.NotFound;

        int?    previousBest  = null;
        string? initialStatus = null;

        if (userId.HasValue)
        {
            var attempts = await dbContext.FraudAttempts
                .AsNoTracking()
                .Where(a => !a.IsDeleted && a.UserId == userId.Value && a.ScenarioId == id)
                .Select(a => new { a.Score, a.Passed })
                .ToListAsync(ct);

            if (attempts.Count > 0)
            {
                previousBest  = attempts.Max(a => a.Score);
                initialStatus = attempts.Any(a => a.Passed) ? "completed" : "failed";
            }
        }

        return MapToResponse(scenario, previousBest, initialStatus);
    }

    private static FraudScenarioResponse MapToResponse(
        Domain.Entities.FraudScenario s,
        int?    previousBest,
        string? initialStatus)
    {
        return new FraudScenarioResponse
        {
            Id               = s.Id,
            Title            = s.Title,
            Description      = s.Description,
            FraudType        = EnumToString(s.FraudType),
            Difficulty       = EnumToString(s.Difficulty),
            RiskLevel        = EnumToString(s.RiskLevel),
            EstimatedMinutes = s.EstimatedMinutes,
            PassScore        = s.PassScore,
            AverageScore     = s.AverageScore,
            AttemptsCount    = s.AttemptsCount,
            UpdatedAt        = (s.UpdatedAt ?? s.CreatedAt).ToString("yyyy-MM-dd"),
            LearnerRole      = s.LearnerRole,
            PreviousBest     = previousBest,
            InitialStatus    = initialStatus,
            PlayUrl          = s.PlayUrl,
        };
    }

    private static string EnumToString(FraudSimType t) => t switch
    {
        FraudSimType.Phishing          => "phishing",
        FraudSimType.Transaction       => "transaction",
        FraudSimType.Document          => "document",
        FraudSimType.DeepfakeCall      => "deepfake_call",
        FraudSimType.SocialEngineering => "social_engineering",
        FraudSimType.AmlKyc            => "aml_kyc",
        _                              => t.ToString().ToLower(),
    };

    private static string EnumToString(FraudSimDifficulty d) => d switch
    {
        FraudSimDifficulty.Beginner     => "beginner",
        FraudSimDifficulty.Intermediate => "intermediate",
        FraudSimDifficulty.Advanced     => "advanced",
        _                               => d.ToString().ToLower(),
    };

    private static string EnumToString(FraudSimRisk r) => r switch
    {
        FraudSimRisk.Low    => "low",
        FraudSimRisk.Medium => "medium",
        FraudSimRisk.High   => "high",
        _                   => r.ToString().ToLower(),
    };
}
