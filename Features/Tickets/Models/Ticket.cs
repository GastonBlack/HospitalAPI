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
    public required Medic Medic { get; set; }

    public int PatientId { get; set; }
    public required Patient Patient { get; set; }

    public string Status { get; set; } = TicketStatuses.Pending;
}
