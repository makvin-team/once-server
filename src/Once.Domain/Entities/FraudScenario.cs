using System.ComponentModel.DataAnnotations.Schema;
using Once.Domain.Entities.Common;
using Once.Domain.Enums;

namespace Once.Domain.Entities;

public class FraudScenario : AuditableModelBase<long>
{
    [Column(TypeName = "jsonb")]
    public required MultiLanguageField Title { get; set; }

    [Column(TypeName = "jsonb")]
    public required MultiLanguageField Description { get; set; }

    public FraudSimType       FraudType  { get; set; }
    public FraudSimDifficulty Difficulty { get; set; }
    public FraudSimRisk       RiskLevel  { get; set; }

    public int EstimatedMinutes { get; set; }
    public int PassScore        { get; set; }
    public int AverageScore     { get; set; }
    public int AttemptsCount    { get; set; }

    [Column(TypeName = "jsonb")]
    public required MultiLanguageField LearnerRole { get; set; }

    public string? PlayUrl { get; set; }

    public List<FraudAttempt> Attempts { get; set; } = new();
}
