namespace Application.Extensions;

public static class CollectionExtensions
{
    /// <summary>
    /// Koleksiyonun null veya boş olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }

    /// <summary>
    /// Koleksiyonun belirtilen minimum sayıda elemana sahip olup olmadığını kontrol eder.
    /// </summary>
    public static bool HasMinimumCount<T>(this IEnumerable<T> collection, int minCount)
    {
        return collection != null && collection.Count() >= minCount;
    }

    /// <summary>
    /// Koleksiyonun belirtilen maksimum sayıda elemana sahip olup olmadığını kontrol eder.
    /// </summary>
    public static bool HasMaximumCount<T>(this IEnumerable<T> collection, int maxCount)
    {
        return collection != null && collection.Count() <= maxCount;
    }
}