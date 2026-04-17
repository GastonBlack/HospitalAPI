using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Features.Medics.DTOs;

public class CreateMedicDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(8, MinimumLength = 7)]
    public string Document { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Specialty { get; set; } = string.Empty;
}
