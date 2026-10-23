namespace Domain.Exceptions;

public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base("İş kuralı hatası : " + message) { }
}
