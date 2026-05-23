namespace Once.Domain.Abstractions;

public record Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);
}

public record Result<T> : Result
{
    public Result(
        bool isSuccess,
        Error error,
        T? data = default) : base(isSuccess, error)
    {
        Data = data;
    }

    public T? Data { get; set; }

    public static Result<T> Success(T data) => new(true, Error.None, data);
    public new static Result<T> Success() => new(true, Error.None, default);
    public new static Result<T> Failure(Error error) => new(false, error, default);

    public static implicit operator Result<T>(T data) => Success(data);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
