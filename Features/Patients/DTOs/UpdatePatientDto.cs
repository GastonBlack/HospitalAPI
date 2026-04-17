using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Features.Patients.DTOs;

public class UpdatePatientDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(8, MinimumLength = 7)]
    [RegularExpression(@"^\d+$", ErrorMessage = "El documento solo puede contener numeros.")]
    public string Document { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
