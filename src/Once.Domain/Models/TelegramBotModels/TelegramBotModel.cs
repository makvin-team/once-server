namespace Once.Domain.Models.TelegramBotModels;

public class TelegramBotModel
{
    public TelegramTopicType Type { get; set; }

    public long? ScoringId { get; set; }
    public long? Tin { get; set; }
    public string? ClientName { get; set; }

    public string Message { get; set; } = default!;
    public DateTime OccurredAt { get; set; } = DateTime.Now;

    public string? BankName { get; set; }
    public long? BankTin { get; set; }
    public string? ApplicationNumber { get; set; }
    public string? ScoringResult { get; set; }
    public string? PdfUrl { get; set; }

    public Dictionary<string, object?>? ResultDetails { get; set; }
}

public enum TelegramTopicType
{
    ScoringFailed = 1,
    ScoringCompleted = 2,
    TechnicalError = 3,
    SoliqError = 4,
    RegionError = 5,
    KatmError = 6,
    ImproperFinancialUsageError = 7,
    BlackListError = 8,
    IHamkorError = 9,
}
