namespace Application.Features.AppUserFeature.DTOs;

public class LoginResponseDto
{
    public AppUserDto User { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime Expiration { get; set; }
}
