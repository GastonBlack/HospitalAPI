using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Features.Medics.DTOs;

public class DisableMedicDto
{
    [Required]
    public bool IsActive { get; set; }
}
