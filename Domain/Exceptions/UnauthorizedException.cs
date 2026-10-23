namespace Domain.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Kimlik doğrulama başarısız.") : base(message) { }
}