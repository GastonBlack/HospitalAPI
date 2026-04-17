using HospitalAPI.Features.Medics.DTOs;

namespace HospitalAPI.Features.Medics.IServices;

public interface IMedicService
{
    Task<List<GetMedicDto>> GetAllAsync();
    Task<GetMedicDto?> GetByIdAsync(int id);
    Task<ResponseMedicDto> CreateAsync(CreateMedicDto dto);
    Task<ResponseMedicDto?> UpdateAsync(int id, UpdateMedicDto dto);
    Task<bool> ChangeActiveStatusAsync(int id, DisableMedicDto dto);
}
