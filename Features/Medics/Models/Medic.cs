using HospitalAPI.Features.Medics.Constants;
using HospitalAPI.Features.Tickets.Models;

namespace HospitalAPI.Features.Medics.Models;

public class Medic
{
    public int Id { get; set; }

    // Name and LastName build FullName.
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string FullName
    {
        get { return Name + " " + LastName; }
    }

    public required string Document { get; set; }
    public string Role { get; set; } = "Medic";

    public bool IsActive { get; set; } = true;

    public string Specialty { get; set; } = MedicSpecialties.NoSpecialty;

    public List<Ticket> Tickets { get; set; } = [];
}
