namespace HospitalAPI.Features.Tickets.DTOs;

public class ResponseTicketDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime CreationDate { get; set; }
    public int MedicId { get; set; }
    public int PatientId { get; set; }
    public string? Status { get; set; }
}
