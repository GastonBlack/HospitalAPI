using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Features.Auth.DTOs;

public class LoginDto
{
    [Required]
    [StringLength(8, MinimumLength = 7)]
    [RegularExpression(@"^\d+$", ErrorMessage = "El documento solo puede contener numeros.")]
    public string Document { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
