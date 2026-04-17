namespace HospitalAPI.Features.Medics.DTOs;

public class GetMedicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
