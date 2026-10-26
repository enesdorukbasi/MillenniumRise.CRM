namespace Application.Extensions;

public static class ObjectValidationExtensions
{
    /// <summary>
    /// Nesnenin null olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsNull(this object obj)
    {
        return obj == null;
    }

    /// <summary>
    /// Nesnenin null olmadığını kontrol eder.
    /// </summary>
    public static bool IsNotNull(this object obj)
    {
        return obj != null;
    }

    /// <summary>
    /// Nesnenin belirtilen türde olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsOfType<T>(this object obj)
    {
        return obj is T;
    }
}