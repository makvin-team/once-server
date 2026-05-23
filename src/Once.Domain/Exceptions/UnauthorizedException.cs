namespace Once.Domain.Exceptions;

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException() : base("Unauthorized")
    {
    }

    public UnauthorizedException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public UnauthorizedException(Exception exception) : base(exception)
    {
    }

    public override int StatusCode => 401;
}
