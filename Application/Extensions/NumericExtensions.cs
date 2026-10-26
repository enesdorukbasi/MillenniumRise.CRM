namespace Application.Extensions;

public static class NumericExtensions
{
    /// <summary>
    /// Sayının pozitif olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsPositive(this int number)
    {
        return number > 0;
    }

    /// <summary>
    /// Sayının negatif olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsNegative(this int number)
    {
        return number < 0;
    }

    /// <summary>
    /// Sayının belirtilen aralıkta olup olmadığını kontrol eder (dahil).
    /// </summary>
    public static bool IsInRange(this int number, int min, int max)
    {
        return number >= min && number <= max;
    }

    /// <summary>
    /// Double değerin pozitif olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsPositive(this double number)
    {
        return number > 0;
    }

    /// <summary>
    /// Double değerin negatif olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsNegative(this double number)
    {
        return number < 0;
    }

    /// <summary>
    /// Double değerin belirtilen aralıkta olup olmadığını kontrol eder (dahil).
    /// </summary>
    public static bool IsInRange(this double number, double min, double max)
    {
        return number >= min && number <= max;
    }

    /// <summary>
    /// Gelen decimal değeri para birimine uygun hale getirir.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static decimal MoneyRound2(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);
}