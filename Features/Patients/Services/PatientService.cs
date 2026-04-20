using HospitalAPI.Features.Patients.DTOs;
using HospitalAPI.Features.Patients.IServices;
using HospitalAPI.Features.Patients.Models;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HospitalAPI.Features.Patients.Services;

public class PatientService : IPatientService
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly HospitalDbContext _db;
    public PatientService(HospitalDbContext db)
    {
        _db = db;
    }

    // //////////////////////////////////////////
    // Validations.
    // //////////////////////////////////////////
    private async Task<bool> DocumentAlreadyExistsAsync(string document, int? excludedPatientId = null)
    {
        return await _db.Patients.AnyAsync(p =>
                   p.Document == document &&
                   (!excludedPatientId.HasValue || p.Id != excludedPatientId.Value)) ||
               await _db.Admins.AnyAsync(a => a.Document == document) ||
               await _db.Medics.AnyAsync(m => m.Document == document);
    }

    private static ResponsePatientDto MapToResponsePatientDto(Patient patient)
    {
        return new ResponsePatientDto
        {
            Id = patient.Id,
            Name = patient.Name,
            LastName = patient.LastName,
            FullName = patient.Name + " " + patient.LastName,
            Document = patient.Document,
            Role = patient.Role,
            IsActive = patient.IsActive
        };
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }

    // //////////////////////////////////////////
    // Getters
    // //////////////////////////////////////////
    public async Task<List<GetPatientDto>> GetAllAsync()
    {
        return await _db.Patients
            .AsNoTracking()
            .OrderByDescending(p => p.Name)
            .Where(p => p.IsActive)
            .Select(p => new GetPatientDto
            {
                Id = p.Id,
                Name = p.Name,
                LastName = p.LastName,
                FullName = p.Name + " " + p.LastName,
                Document = p.Document,
                Role = p.Role,
                IsActive = p.IsActive
            }).ToListAsync();
    }


    public async Task<GetPatientDto?> GetByIdAsync(int id)
    {
        var patient = await _db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) throw new NotFoundException("Usuario no encontrado.");

        return new GetPatientDto
        {
            Id = patient.Id,
            Name = patient.Name,
            LastName = patient.LastName,
            FullName = patient.Name + " " + patient.LastName,
            Document = patient.Document,
            Role = patient.Role,
            IsActive = patient.IsActive
        };
    }

    // //////////////////////////////////////////
    // Mutations
    // //////////////////////////////////////////
    public async Task<ResponsePatientDto> CreateAsync(CreatePatientDto dto)
    {
        if (dto == null) throw new BadRequestException("Datos invalidos.");

        var normalizedName = dto.Name.Trim().ToLower();
        var normalizedLastName = dto.LastName.Trim().ToLower();
        var normalizedDocument = dto.Document.Trim();

        if (await DocumentAlreadyExistsAsync(normalizedDocument))
            throw new ConflictException("Ya existe un usuario con ese documento.");

        Patient newPatient = new()
        {
            Name = normalizedName,
            LastName = normalizedLastName,
            Document = normalizedDocument,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _db.Patients.AddAsync(newPatient);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException("Un usuario con ese documento ya existe.");
        }

        return MapToResponsePatientDto(newPatient);
    }


    public async Task<ResponsePatientDto?> UpdateAsync(int id, UpdatePatientDto dto)
    {
        if (dto == null) throw new BadRequestException("Datos invalidos.");

        Patient patient = await _db.Patients.FindAsync(id)
            ?? throw new NotFoundException("El usuario no existe.");

        var normalizedName = dto.Name.Trim().ToLower();
        var normalizedLastName = dto.LastName.Trim().ToLower();
        var normalizedDocument = dto.Document.Trim();

        bool samePassword = BCrypt.Net.BCrypt.Verify(dto.Password, patient.PasswordHash);

        bool sameData =
            normalizedName == patient.Name &&
            normalizedLastName == patient.LastName &&
            normalizedDocument == patient.Document &&
            samePassword;

        if (sameData)
            throw new BadRequestException("Ingrese datos diferentes a los actuales.");

        if (await DocumentAlreadyExistsAsync(normalizedDocument, id))
            throw new ConflictException("Ya existe un usuario con ese documento.");

        patient.Name = normalizedName;
        patient.LastName = normalizedLastName;
        patient.Document = normalizedDocument;
        patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException("Ya existe un usuario con ese documento.");
        }

        return MapToResponsePatientDto(patient);
    }


    public async Task<bool> ChangeActiveStatusAsync(int id, DisablePatientDto dto)
    {
        if (dto == null) throw new BadRequestException("Datos invalidos.");

        int affectedRows = await _db.Patients
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.IsActive, dto.IsActive));

        if (affectedRows == 0)
            throw new NotFoundException("El usuario no existe.");

        return true;
    }
}
