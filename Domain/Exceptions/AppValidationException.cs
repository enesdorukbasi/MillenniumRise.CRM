namespace Domain.Exceptions;

public sealed class AppValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }
    
    public AppValidationException(string error)
        : base(string.IsNullOrWhiteSpace(error) ? "Doğrulama hataları oluştu." : error)
    {
        Errors = new List<string> { error }.AsReadOnly();
    }

    public AppValidationException(IEnumerable<string> errors)
        : base(BuildMessage(errors))
    {
        Errors = errors.ToList().AsReadOnly();
    }

    private static string BuildMessage(IEnumerable<string> errors)
    {
        var list = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? new List<string>();
        if (!list.Any())
        {
            return "Doğrulama hataları oluştu.";
        }
        return string.Join("\n", list);
    }

    public override string ToString()
    {
        return string.Join("\n", Errors);
    }
}
