using Application.Abstractions.Validators;

namespace Application.Common.Validators;

public abstract class BaseValidator<T> : IValidator<T> where T : class
{
    private readonly List<string> _errors = new();

    public List<string> GetErrors => _errors.ToList();
    public bool IsValid => !_errors.Any();

    public void ClearErrors() => _errors.Clear();

    protected void AddError(string error)
    {
        if (!string.IsNullOrWhiteSpace(error))
            _errors.Add(error);
    }

    public abstract bool Validate(T model);
}