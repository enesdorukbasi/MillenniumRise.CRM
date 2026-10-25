using Application.Common.DTOs;

namespace Application.Feature.AppUserFeature.DTOs;

public record AppUserDto : BaseDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public string? LastLoginIp { get; set; }
    public string? LastLoginDevice { get; set; }
}
