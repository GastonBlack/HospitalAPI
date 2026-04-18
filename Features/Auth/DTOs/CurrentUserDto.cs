namespace HospitalAPI.Features.Auth.DTOs;

public class CurrentUserDto
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
