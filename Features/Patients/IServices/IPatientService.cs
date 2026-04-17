using HospitalAPI.Features.Patients.DTOs;

namespace HospitalAPI.Features.Patients.IServices;

public interface IPatientService
{
    Task<List<GetPatientDto>> GetAllAsync();
    Task<GetPatientDto?> GetByIdAsync(int id);
    Task<ResponsePatientDto> CreateAsync(CreatePatientDto dto);
    Task<ResponsePatientDto?> UpdateAsync(int id, UpdatePatientDto dto);
    Task<bool> ChangeActiveStatusAsync(int id, DisablePatientDto dto);
}