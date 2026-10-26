namespace Application.Abstractions.Validators;

public interface IValidator<T>
{
    bool Validate(T model);
    List<string> GetErrors { get; }
    bool IsValid { get; }
    void ClearErrors();
}
