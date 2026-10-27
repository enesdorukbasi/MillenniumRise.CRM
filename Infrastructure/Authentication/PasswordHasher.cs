using Application.Abstractions.Authentication;
using System.Globalization;
using System.Security.Cryptography;

namespace Infrastructure.Authentication;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int IterationCount = 210_000;
    private const string AlgorithmName = "PBKDF2-SHA512";

    public (string Hash, string SaltBase64) Hash(string password, string? saltBase64 = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = saltBase64 is null
            ? RandomNumberGenerator.GetBytes(SaltSize)
            : Convert.FromBase64String(saltBase64);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            IterationCount,
            HashAlgorithmName.SHA512,
            HashSize);

        return ($"{AlgorithmName}${IterationCount}${Convert.ToBase64String(hash)}", Convert.ToBase64String(salt));
    }

    public bool Verify(string password, string storedHash, string storedSaltBase64)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(storedHash) ||
            string.IsNullOrWhiteSpace(storedSaltBase64))
        {
            return false;
        }

        var parts = storedHash.Split('$');

        if (parts.Length != 3 ||
            !string.Equals(parts[0], AlgorithmName, StringComparison.Ordinal) ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var iterationCount) ||
            iterationCount <= 0)
        {
            return false;
        }

        try
        {
            var expectedHash = Convert.FromBase64String(parts[2]);
            var salt = Convert.FromBase64String(storedSaltBase64);
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterationCount,
                HashAlgorithmName.SHA512,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public bool NeedsRehash(string storedHash)
    {
        var parts = storedHash.Split('$');

        return parts.Length != 3 ||
               !string.Equals(parts[0], AlgorithmName, StringComparison.Ordinal) ||
               !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var iterationCount) ||
               iterationCount < IterationCount;
    }
}
