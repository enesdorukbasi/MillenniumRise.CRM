namespace Application.Abstractions.Persistence.Seeders.Models;

public sealed class AdminUserSettings
{
    public const string SectionName = "AdminUser";

    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
