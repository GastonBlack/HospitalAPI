using HospitalAPI.Features.Medics.Models;
using HospitalAPI.Features.Patients.Models;
using HospitalAPI.Features.Tickets.Constants;

namespace HospitalAPI.Features.Tickets.Models;

public class Ticket
{
    public int Id { get; set; }

    public required DateTime AppointmentDate { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public int MedicId { get; set; }
    public Medic Medic { get; set; } = null!;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public string Status { get; set; } = TicketStatuses.Pending;
}
