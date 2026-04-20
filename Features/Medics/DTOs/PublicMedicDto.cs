namespace HospitalAPI.Features.Medics.DTOs;

public class PublicMedicDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}
