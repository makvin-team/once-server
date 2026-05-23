using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Once.Domain.Entities.Common;
using Once.Domain.Enums;

namespace Once.Domain.Entities;

public class FraudScenario : AuditableModelBase<long>
{
    [MaxLength(255)]
    public required string Title { get; set; }

    public required string Description { get; set; }

    public FraudSimType     FraudType  { get; set; }
    public FraudSimDifficulty Difficulty { get; set; }
    public FraudSimRisk     RiskLevel  { get; set; }

    public int EstimatedMinutes { get; set; }
    public int PassScore        { get; set; }
    public int AverageScore     { get; set; }
    public int AttemptsCount    { get; set; }

    [Column(TypeName = "jsonb")]
    public required string Skills { get; set; }

    public required string Context      { get; set; }
    public required string LearnerRole  { get; set; }
    public required string Task         { get; set; }
    public required string Explanation  { get; set; }
    public required string Recommendation { get; set; }

    [Column(TypeName = "jsonb")]
    public required string Evidence { get; set; }

    [Column(TypeName = "jsonb")]
    public required string RedFlagOptions { get; set; }

    [Column(TypeName = "jsonb")]
    public required string DecisionOptions { get; set; }

    public List<FraudAttempt> Attempts { get; set; } = new();
}
