using System.ComponentModel.DataAnnotations;
using HospitalAPI.Features.Tickets.Constants;

namespace HospitalAPI.Features.Tickets.DTOs;

public class CreateTicketDto
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MedicId { get; set; }

    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = TicketStatuses.Pending;
}
