using System.ComponentModel.DataAnnotations.Schema;
using Once.Domain.Entities.Common;
using Once.Domain.Enums;

namespace Once.Domain.Entities;

public class FraudAttempt : AuditableModelBase<long>
{
    [Column("scenario_id")]
    public long ScenarioId { get; set; }

    [ForeignKey(nameof(ScenarioId))]
    public FraudScenario Scenario { get; set; } = null!;

    [Column("user_id")]
    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int  Score         { get; set; }
    public bool Passed        { get; set; }
    public int  DetectedFlags { get; set; }
    public int  MissedFlags   { get; set; }

    public FraudSimStatus Status { get; set; }

    [Column(TypeName = "jsonb")]
    public string SelectedFlagIds    { get; set; } = "[]";
    public string SelectedDecisionId { get; set; } = "";
}
