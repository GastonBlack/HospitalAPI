using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Features.Tickets.DTOs;

public class CreateTicketDto
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MedicId { get; set; }
}
