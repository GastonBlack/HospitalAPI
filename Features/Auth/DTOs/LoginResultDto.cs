namespace HospitalAPI.Features.Auth.DTOs;

public class LoginResultDto
{
    public string Token { get; set; } = string.Empty;
    public CurrentUserDto User { get; set; } = new();
    public DateTime ExpiresAtUtc { get; set; }
}
