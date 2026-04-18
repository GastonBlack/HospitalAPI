using System.Data;
using HospitalAPI.Features.Medics.Models;
using HospitalAPI.Features.Tickets.Constants;
using HospitalAPI.Features.Tickets.DTOs;
using HospitalAPI.Features.Tickets.IServices;
using HospitalAPI.Features.Tickets.Models;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HospitalAPI.Features.Tickets.Services;

public class TicketService : ITicketService
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly HospitalDbContext _db;
    public TicketService(HospitalDbContext db)
    {
        _db = db;
    }

    // //////////////////////////////////////////
    // Helpers
    // //////////////////////////////////////////
    private bool ValidateAppointmentDate(DateTime date)
    {
        // Appointment must be at least 30 minutes from it's creation.
        // Schedule an appointment for a maximum of 2 months from now.
        DateTime now = DateTime.UtcNow;
        return date >= now.AddMinutes(30) && date <= now.AddMonths(2);
    }

    private async Task<bool> ValidateMedicAvailabilityAsync(int medicId, DateTime date, int? excludedTicketId = null)
    {
        return !await _db.Tickets.AsNoTracking().AnyAsync(t =>
            (!excludedTicketId.HasValue || t.Id != excludedTicketId.Value) &&
            t.MedicId == medicId &&
            (t.Status == TicketStatuses.Pending || t.Status == TicketStatuses.Confirmed) &&
            Math.Abs((t.AppointmentDate - date).TotalMinutes) < 30);
    }

    private async Task<bool> ValidatePatientAvailabilityAsync(int patientId, DateTime date, int? excludedTicketId = null)
    {
        return !await _db.Tickets.AsNoTracking().AnyAsync(t =>
            (!excludedTicketId.HasValue || t.Id != excludedTicketId.Value) &&
            t.PatientId == patientId &&
            (t.Status == TicketStatuses.Pending || t.Status == TicketStatuses.Confirmed) &&
            Math.Abs((t.AppointmentDate - date).TotalMinutes) < 30);
    }

    private static bool ValidateTicketStatus(string status)
    {
        return status == TicketStatuses.Pending ||
               status == TicketStatuses.Confirmed ||
               status == TicketStatuses.Completed ||
               status == TicketStatuses.Cancelled;
    }

    private static bool CanChangeSchedule(string status)
    {
        return status == TicketStatuses.Pending || status == TicketStatuses.Confirmed;
    }

    private static bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        if (currentStatus == newStatus) return true;

        return currentStatus switch
        {
            TicketStatuses.Pending =>
                newStatus == TicketStatuses.Confirmed ||
                newStatus == TicketStatuses.Cancelled,

            TicketStatuses.Confirmed =>
                newStatus == TicketStatuses.Completed ||
                newStatus == TicketStatuses.Cancelled,

            TicketStatuses.Completed => false,
            TicketStatuses.Cancelled => false,
            _ => false
        };
    }

    private static bool IsSerializationFailure(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.SerializationFailure;
    }

    private ResponseTicketDto MapToResponseTicketDto(Ticket ticket)
    {
        return new ResponseTicketDto
        {
            Id = ticket.Id,
            AppointmentDate = ticket.AppointmentDate,
            CreationDate = ticket.CreationDate,
            MedicId = ticket.MedicId,
            PatientId = ticket.PatientId,
            Status = ticket.Status
        };
    }

    // //////////////////////////////////////////
    // Getters
    // //////////////////////////////////////////
    public async Task<List<GetTicketDto>> GetAllByMedicIdAsync(int medicId)
    {
        return await _db.Tickets
            .AsNoTracking()
            .OrderBy(t => t.AppointmentDate)
            .Where(t => t.MedicId == medicId)
            .Where(t => t.Status == TicketStatuses.Confirmed)
            .Select(t => new GetTicketDto
            {
                Id = t.Id,
                AppointmentDate = t.AppointmentDate,
                CreationDate = t.CreationDate,
                MedicId = t.MedicId,
                MedicFullName = t.Medic.FullName,
                PatientId = t.PatientId,
                PatientFullName = t.Patient.FullName,
                Status = t.Status
            }).ToListAsync();
    }


    public async Task<List<GetTicketDto>> GetAllByPatientIdAsync(int patientId)
    {
        return await _db.Tickets
            .AsNoTracking()
            .OrderBy(t => t.AppointmentDate)
            .Where(t => t.PatientId == patientId)
            .Where(t => t.Status == TicketStatuses.Confirmed || t.Status == TicketStatuses.Pending)
            .Select(t => new GetTicketDto
            {
                Id = t.Id,
                AppointmentDate = t.AppointmentDate,
                CreationDate = t.CreationDate,
                MedicId = t.MedicId,
                MedicFullName = t.Medic.FullName,
                PatientId = t.PatientId,
                PatientFullName = t.Patient.FullName,
                Status = t.Status
            }).ToListAsync();
    }


    public async Task<List<GetTicketDto>> GetAllAsync()
    {
        return await _db.Tickets
            .AsNoTracking()
            .OrderByDescending(t => t.Id)
            .Select(t => new GetTicketDto
            {
                Id = t.Id,
                AppointmentDate = t.AppointmentDate,
                CreationDate = t.CreationDate,
                MedicId = t.MedicId,
                MedicFullName = t.Medic.FullName,
                PatientId = t.PatientId,
                PatientFullName = t.Patient.FullName,
                Status = t.Status
            }).ToListAsync();
    }


    public async Task<GetTicketDto?> GetByIdAsync(int id)
    {
        var ticket = await _db.Tickets
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new GetTicketDto
            {
                Id = t.Id,
                AppointmentDate = t.AppointmentDate,
                CreationDate = t.CreationDate,
                MedicId = t.MedicId,
                MedicFullName = t.Medic.FullName,
                PatientId = t.PatientId,
                PatientFullName = t.Patient.FullName,
                Status = t.Status
            }).FirstOrDefaultAsync() ?? throw new NotFoundException("No existe el ticket ingresado.");
            
        return ticket;
    }

    // //////////////////////////////////////////
    // Mutations
    // //////////////////////////////////////////
    public async Task<ResponseTicketDto> CreateAsync(int patientId, CreateTicketDto dto)
    {
        if (dto == null) throw new BadRequestException("Los datos ingresados son invalidos.");
        if (!ValidateAppointmentDate(dto.AppointmentDate))
            throw new BadRequestException("El turno debe reservarse con minimo 30 minutos de anticipacion y maximo 2 meses.");

        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var medicTicket = await _db.Medics
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == dto.MedicId && m.IsActive);
            if (medicTicket == null) throw new NotFoundException("No existe el medico seleccionado.");

            var patientTicket = await _db.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == patientId && p.IsActive);
            if (patientTicket == null) throw new NotFoundException("Usuario no existente.");

            bool medicAvailable = await ValidateMedicAvailabilityAsync(dto.MedicId, dto.AppointmentDate);
            if (!medicAvailable) throw new ConflictException("El medico ya tiene un ticket en ese horario.");

            bool patientAvailable = await ValidatePatientAvailabilityAsync(patientId, dto.AppointmentDate);
            if (!patientAvailable) throw new ConflictException("Ya tienes un ticket reservado para esa hora.");

            Ticket newTicket = new()
            {
                PatientId = patientId,
                Patient = patientTicket,
                MedicId = dto.MedicId,
                Medic = medicTicket,
                AppointmentDate = dto.AppointmentDate,
                Status = TicketStatuses.Pending
            };

            await _db.Tickets.AddAsync(newTicket);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapToResponseTicketDto(newTicket);
        }
        catch (DbUpdateException exception) when (IsSerializationFailure(exception))
        {
            await transaction.RollbackAsync();
            throw new ConflictException("Otro usuario reservo ese horario al mismo tiempo. Intenta nuevamente.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<ResponseTicketDto?> UpdateAsync(int id, UpdateTicketDto dto)
    {
        if (dto == null) throw new BadRequestException("Los datos ingresados son invalidos.");
        var normalizedStatus = dto.Status.Trim();

        if (!ValidateTicketStatus(normalizedStatus))
            throw new BadRequestException("El estado del ticket no es valido.");

        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new NotFoundException("No existe el ticket ingresado.");

            if (!IsValidStatusTransition(ticket.Status, normalizedStatus))
                throw new BadRequestException("La transicion de estado del ticket no es valida.");

            bool scheduleChanged =
                ticket.AppointmentDate != dto.AppointmentDate ||
                ticket.MedicId != dto.MedicId ||
                ticket.PatientId != dto.PatientId;

            if (scheduleChanged)
            {
                if (!CanChangeSchedule(ticket.Status))
                    throw new BadRequestException("No se puede modificar la agenda de un ticket cancelado o completado.");

                if (!ValidateAppointmentDate(dto.AppointmentDate))
                    throw new BadRequestException("El turno debe reservarse con minimo 30 minutos de anticipacion y maximo 2 meses.");

                var medicTicket = await _db.Medics
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == dto.MedicId && m.IsActive);
                if (medicTicket == null) throw new NotFoundException("No existe el medico seleccionado.");

                var patientTicket = await _db.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == dto.PatientId && p.IsActive);
                if (patientTicket == null) throw new NotFoundException("No existe el paciente seleccionado.");

                bool medicAvailable = await ValidateMedicAvailabilityAsync(dto.MedicId, dto.AppointmentDate, id);
                if (!medicAvailable) throw new ConflictException("El medico ya tiene un ticket en ese horario.");

                bool patientAvailable = await ValidatePatientAvailabilityAsync(dto.PatientId, dto.AppointmentDate, id);
                if (!patientAvailable) throw new ConflictException("El paciente ya tiene un ticket reservado para esa hora.");
            }

            ticket.AppointmentDate = dto.AppointmentDate;
            ticket.MedicId = dto.MedicId;
            ticket.PatientId = dto.PatientId;
            ticket.Status = normalizedStatus;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseTicketDto
            {
                Id = ticket.Id,
                AppointmentDate = ticket.AppointmentDate,
                CreationDate = ticket.CreationDate,
                MedicId = ticket.MedicId,
                PatientId = ticket.PatientId,
                Status = ticket.Status
            };
        }
        catch (DbUpdateException exception) when (IsSerializationFailure(exception))
        {
            await transaction.RollbackAsync();
            throw new ConflictException("Otro usuario modifico ese horario al mismo tiempo. Intenta nuevamente.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
