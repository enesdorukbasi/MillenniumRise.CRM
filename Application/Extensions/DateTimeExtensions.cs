using Domain.Enums;

namespace Application.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// DateTime değerinin geçmişte olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsInThePast(this DateTime date)
    {
        return date < DateTime.Now;
    }

    /// <summary>
    /// DateTime değerinin gelecekte olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsInTheFuture(this DateTime date)
    {
        return date > DateTime.Now;
    }

    /// <summary>
    /// DateTime değerinin bugüne ait olup olmadığını kontrol eder.
    /// </summary>
    public static bool IsToday(this DateTime date)
    {
        return date.Date == DateTime.Today;
    }

    /// <summary>
    /// DateTime değerinin belirtilen başlangıç ve bitiş tarihleri arasında olup olmadığını kontrol eder (dahil).
    /// </summary>
    public static bool IsBetween(this DateTime date, DateTime start, DateTime end)
    {
        return date >= start && date <= end;
    }
}