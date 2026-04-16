using HospitalAPI.Features.Tickets.Models;

namespace HospitalAPI.Features.Patients.Models;

public class Patient
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
    public string Role { get; set; } = "Patient";

    public List<Ticket> Tickets { get; set; } = [];
}
