using Domain.Base;

namespace Domain.Entities;

public sealed class AppUser : BaseEntity
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string HashedPassword { get; set; }
    public required string PasswordSalt { get; set; }

    public DateTimeOffset? LastLoginDate { get; set; }
    public string? LastLoginIp { get; set; }
    public string? LastLoginDevice { get; set; }
}
