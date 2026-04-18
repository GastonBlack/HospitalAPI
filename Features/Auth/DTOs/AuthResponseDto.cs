namespace HospitalAPI.Features.Auth.DTOs;

public class AuthResponseDto
{
    public CurrentUserDto User { get; set; } = new();
    public DateTime ExpiresAtUtc { get; set; }
}
