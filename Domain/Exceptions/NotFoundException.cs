namespace Domain.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException() : base("Aradığınız niteliklerde bir değer bulunamadı.") { }
    public NotFoundException(string entity, object key) : base($"{entity}'e ait ({key}) bulunamadı.") { }
}
