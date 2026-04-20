using HospitalAPI.Features.Medics.Constants;
using HospitalAPI.Features.Tickets.Models;
using HospitalAPI.Infrastructure.Auth;

namespace HospitalAPI.Features.Medics.Models;

public class Medic
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{Name} {LastName}";

    public required string Document { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = AuthRoles.Medic;

    public bool IsActive { get; set; } = true;

    public string Specialty { get; set; } = MedicSpecialties.NoSpecialty;

    public List<Ticket> Tickets { get; set; } = [];
}
