using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Features.Patients.DTOs;

public class DisablePatientDto
{
    [Required]
    public bool IsActive { get; set; }
}