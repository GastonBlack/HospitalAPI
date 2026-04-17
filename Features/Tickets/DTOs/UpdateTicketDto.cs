using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Features.Tickets.DTOs;

public class UpdateTicketDto
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MedicId { get; set; }

    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;
}
