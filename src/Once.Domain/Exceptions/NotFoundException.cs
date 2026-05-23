namespace Once.Domain.Exceptions;

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override int StatusCode => 404;

    public static void ThrowIfNull(object? data, string message = "Not found")
    {
        if (data is null) throw new NotFoundException(message);
    }
}
