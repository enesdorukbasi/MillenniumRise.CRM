using System.Text.RegularExpressions;

namespace Application.Extensions;

public static class StringExtension
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex InternationalPhoneNumberRegex = new(@"^\+?\d{7,15}$", RegexOptions.Compiled);
    private static readonly Regex UrlRegex = new(@"^(http|https)://([\w-]+(\.[\w-]+)+)([/#?]?.*)$", RegexOptions.Compiled);
    private static readonly Regex AlphaNumericRegex = new(@"^[a-zA-Z0-9]*$", RegexOptions.Compiled);
    private static readonly Regex IpAddressRegex = new(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", RegexOptions.Compiled);

    /// <summary>
    /// Boş mu veya null mu?
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Boş mu veya null mu?
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNotNullOrEmpty(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Email'in geçerli olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    /// <summary>
    /// Telefon numarasının geçerli olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public static bool IsValidPhoneNumber(this string? phoneNumber)
    {
        var normalizedPhoneNumber = phoneNumber.NormalizePhoneNumber();
        return normalizedPhoneNumber is not null && InternationalPhoneNumberRegex.IsMatch(normalizedPhoneNumber);
    }

    /// <summary>
    /// Telefon numarasını uluslararası saklama formatına dönüştürür.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public static string? NormalizePhoneNumber(this string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        var trimmedPhoneNumber = phoneNumber.Trim();
        var hasPlusPrefix = trimmedPhoneNumber.StartsWith('+');
        var hasDoubleZeroPrefix = trimmedPhoneNumber.StartsWith("00", StringComparison.Ordinal);
        var digits = new string(trimmedPhoneNumber.Where(char.IsDigit).ToArray());

        if (hasDoubleZeroPrefix)
        {
            if (digits.Length <= 2)
                return null;

            digits = digits[2..];
            hasPlusPrefix = true;
        }

        if (digits.Length < 7 || digits.Length > 15)
            return null;

        return hasPlusPrefix ? $"+{digits}" : digits;
    }

    /// <summary>
    /// Url adresinin geçerli olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool IsValidUrl(this string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return UrlRegex.IsMatch(url);
    }

    /// <summary>
    /// Text'in uzunluğunu kontrol eder.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static bool HasLength(this string value, int length)
    {
        if (value.IsNullOrEmpty())
            return false;

        return value.Length == length;
    }

    /// <summary>
    /// Text'in minimum ve maximum uzunluğunu kontrol eder.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minLength"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static bool IsLengthBetween(this string value, int minLength, int maxLength)
    {
        if (value.IsNullOrEmpty())
            return false;

        return value.Length >= minLength && value.Length <= maxLength;
    }

    /// <summary>
    /// Text'in minimum uzunluğunu kontrol eder.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="minLength"></param>
    /// <returns></returns>
    public static bool MinLengthControl(this string text, int minLength)
    {
        return text.Length >= minLength;
    }

    /// <summary>
    /// Text'in maksimum uzunluğunu kontrol eder.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static bool MaxLengthControl(this string? text, int maxLength)
    {
        return text is not null && text.Length <= maxLength;
    }

    /// <summary>
    /// Değer alfanumerik bir değer mi?
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsAlphaNumeric(this string value)
    {
        if (value.IsNullOrEmpty())
            return false;

        return AlphaNumericRegex.IsMatch(value);
    }

    /// <summary>
    /// string formatında ki date geçerli mi?
    /// </summary>
    /// <param name="date"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static bool IsValidDate(this string date, string format = "yyyy-MM-dd")
    {
        if (date.IsNullOrEmpty())
            return false;

        return DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out _);
    }

    /// <summary>
    /// Değer '-' mi '+' mı?
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsPositiveNumber(this string value)
    {
        if (value.IsNullOrEmpty())
            return false;

        return double.TryParse(value, out double number) && number > 0;
    }

    /// <summary>
    /// Değer tam sayı mı?
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsInteger(this string value)
    {
        if (value.IsNullOrEmpty())
            return false;

        return int.TryParse(value, out _);
    }

    /// <summary>
    /// Değer para mı?
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsMoney(this decimal value)
    {
        if (value < 0) return false;
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
        return decimalPlaces <= 2;
    }

    /// <summary>
    /// Ip adresi geçerli mi?
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public static bool IsValidIpAddress(this string ipAddress)
    {
        if (ipAddress.IsNullOrEmpty())
            return false;

        return IpAddressRegex.IsMatch(ipAddress);
    }

    /// <summary>
    /// Parola uygun mu?
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool IsValidPassword(this string password)
    {
        if (password.IsNullOrEmpty())
            return false;

        return PasswordRegex.IsMatch(password);
    }
}
