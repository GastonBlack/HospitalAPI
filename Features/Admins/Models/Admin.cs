using HospitalAPI.Infrastructure.Auth;

namespace HospitalAPI.Features.Admins.Models;

public class Admin
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{Name} {LastName}";

    public required string Document { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = AuthRoles.Admin;

    public bool IsActive { get; set; } = true;
}
