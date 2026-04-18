using HospitalAPI.Features.Tickets.DTOs;

namespace HospitalAPI.Features.Tickets.IServices;

public interface ITicketService
{
    Task<List<GetTicketDto>> GetAllAsync();
    Task<List<GetTicketDto>> GetAllByMedicIdAsync(int medicId);
    Task<List<GetTicketDto>> GetAllByPatientIdAsync(int patientId);
    Task<GetTicketDto?> GetByIdAsync(int id);
    Task<ResponseTicketDto> CreateAsync(int patientId, CreateTicketDto dto);
    Task<ResponseTicketDto?> UpdateAsync(int id, UpdateTicketDto dto);
}