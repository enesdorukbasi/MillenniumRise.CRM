namespace Application.Feature.AppUserFeature.DTOs;

public class LoginResponseDto
{
    public AppUserDto User { get; set; }
    public string Token { get; set; } = default!;
    public string Role { get; set; }
    public DateTime Expiration { get; set; }
}
