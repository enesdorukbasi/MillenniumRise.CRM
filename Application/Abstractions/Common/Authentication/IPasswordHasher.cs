namespace Application.Abstractions.Platform.Authentication;

public interface IPasswordHasher
{
    (string Hash, string SaltBase64) Hash(string password, string? saltBase64 = null);
    bool Verify(string password, string storedHash, string storedSaltBase64);
    bool NeedsRehash(string storedHash);
}
