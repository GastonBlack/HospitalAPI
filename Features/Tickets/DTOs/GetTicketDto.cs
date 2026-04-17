namespace HospitalAPI.Features.Tickets.DTOs;

public class GetTicketDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime CreationDate { get; set; }
    public int MedicId { get; set; }
    public string MedicFullName { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
