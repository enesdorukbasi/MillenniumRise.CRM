namespace Domain.Exceptions;

public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Bu işlem için yetkiniz yok.") : base(message) { }
}
