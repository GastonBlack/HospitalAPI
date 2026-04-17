using HospitalAPI.Features.Tickets.Models;

namespace HospitalAPI.Features.Patients.Models;

public class Patient
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{Name} {LastName}";

    public required string Document { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "Patient";

    public bool IsActive { get; set; } = true;

    public List<Ticket> Tickets { get; set; } = [];
}
