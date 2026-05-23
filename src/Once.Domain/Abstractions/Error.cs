namespace Once.Domain.Abstractions;

public record Error
{
    private Error()
    {
        Code = "";
        Type = ErrorType.Failure;
        Message = "";
    }
    private Error(string code, ErrorType type, string message = "")
    {
        Code = code;
        Type = type;
        Message = message;
    }

    public static readonly Error None = new(
        string.Empty,
        ErrorType.Failure);

    public static readonly Error NulValue = new(
        "Error.NullValue",
        ErrorType.Failure);

    public static readonly Error InternalServerError = new(
        "Error.InternalServerError",
        ErrorType.Failure);

    public static Error Failure(string code, string message = "") =>
    new(code, ErrorType.Failure, message);

    private Error(
        string code,
        ErrorType errorType)
    {
        Code = code;
        Type = errorType;
        Message = string.Empty;
    }

    public string Code { get; }

    public ErrorType Type { get; }

    public string Message { get; }

    public static Error NotFound(string code) =>
        new(code, ErrorType.NotFound);

    public static Error Validation(string code) =>
        new(code, ErrorType.Validation);

    public static Error Conflict(string code) =>
        new(code, ErrorType.Conflict);

    public static Error Failure(string code) =>
        new(code, ErrorType.Failure);

    public static Error Processing(string code, string message = "") =>
        new(code, ErrorType.Processing, message);
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Processing = 202
}