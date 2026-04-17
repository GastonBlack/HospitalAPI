using HospitalAPI.Features.Medics.DTOs;
using HospitalAPI.Features.Medics.IServices;
using HospitalAPI.Features.Medics.Models;
using HospitalAPI.Features.Medics.Constants;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HospitalAPI.Features.Medics.Services;

public class MedicService : IMedicService
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly HospitalDbContext _db;
    public MedicService(HospitalDbContext db)
    {
        _db = db;
    }

    // //////////////////////////////////////////
    // Validations.
    // //////////////////////////////////////////
    private async Task<bool> MedicExistsByDocumentAsync(string document)
    {
        return await _db.Medics.AnyAsync(m => m.Document == document);
    }

    private static bool IsValidSpecialty(string specialty)
    {
        return MedicSpecialties.All.Contains(specialty);
    }

    private static ResponseMedicDto MapToResponseMedicDto(Medic medic)
    {
        return new ResponseMedicDto
        {
            Id = medic.Id,
            Name = medic.Name,
            LastName = medic.LastName,
            FullName = medic.Name + " " + medic.LastName,
            Document = medic.Document,
            Role = medic.Role,
            Specialty = medic.Specialty,
            IsActive = medic.IsActive
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
    public async Task<List<GetMedicDto>> GetAllAsync()
    {
        return await _db.Medics
            .AsNoTracking()
            .OrderByDescending(m => m.Name)
            .Where(m => m.IsActive)
            .Select(m => new GetMedicDto
            {
                Id = m.Id,
                Name = m.Name,
                LastName = m.LastName,
                FullName = m.Name + " " + m.LastName,
                Document = m.Document,
                Role = m.Role,
                Specialty = m.Specialty,
                IsActive = m.IsActive
            })
            .ToListAsync();
    }

    public async Task<GetMedicDto?> GetByIdAsync(int id)
    {
        var medic = await _db.Medics
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (medic == null) throw new NotFoundException("Medico no encontrado.");

        return new GetMedicDto
        {
            Id = medic.Id,
            Name = medic.Name,
            LastName = medic.LastName,
            FullName = medic.Name + " " + medic.LastName,
            Document = medic.Document,
            Role = medic.Role,
            Specialty = medic.Specialty,
            IsActive = medic.IsActive
        };
    }

    // //////////////////////////////////////////
    // Mutations
    // //////////////////////////////////////////
    public async Task<ResponseMedicDto> CreateAsync(CreateMedicDto dto)
    {
        if (dto == null) throw new BadRequestException("Datos invalidos.");

        var normalizedName = dto.Name.Trim().ToLower();
        var normalizedLastName = dto.LastName.Trim().ToLower();
        var normalizedDocument = dto.Document.Trim();
        var normalizedSpecialty = dto.Specialty.Trim();

        if (!IsValidSpecialty(normalizedSpecialty))
            throw new BadRequestException("La especialidad no es valida.");

        if (await MedicExistsByDocumentAsync(normalizedDocument))
            throw new ConflictException("Un medico con ese documento ya existe.");

        Medic newMedic = new()
        {
            Name = normalizedName,
            LastName = normalizedLastName,
            Document = normalizedDocument,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Specialty = normalizedSpecialty
        };

        await _db.Medics.AddAsync(newMedic);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException("Un medico con ese documento ya existe.");
        }

        return MapToResponseMedicDto(newMedic);
    }

    public async Task<ResponseMedicDto?> UpdateAsync(int id, UpdateMedicDto dto)
    {
        if (dto == null) throw new BadRequestException("Datos invalidos.");

        Medic medic = await _db.Medics.FindAsync(id)
            ?? throw new NotFoundException("El medico no existe.");

        var normalizedName = dto.Name.Trim().ToLower();
        var normalizedLastName = dto.LastName.Trim().ToLower();
        var normalizedDocument = dto.Document.Trim();
        var normalizedSpecialty = dto.Specialty.Trim();

        if (!IsValidSpecialty(normalizedSpecialty))
            throw new BadRequestException("La especialidad no es valida.");

        bool samePassword = BCrypt.Net.BCrypt.Verify(dto.Password, medic.PasswordHash);

        bool sameData =
            normalizedName == medic.Name &&
            normalizedLastName == medic.LastName &&
            normalizedDocument == medic.Document &&
            normalizedSpecialty == medic.Specialty &&
            samePassword;

        if (sameData)
            throw new BadRequestException("Ingrese datos diferentes a los actuales.");

        bool documentInUse = await _db.Medics
            .AnyAsync(m => m.Document == normalizedDocument && m.Id != id);

        if (documentInUse)
            throw new ConflictException("Un medico con ese documento ya existe.");

        medic.Name = normalizedName;
        medic.LastName = normalizedLastName;
        medic.Document = normalizedDocument;
        medic.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        medic.Specialty = normalizedSpecialty;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException("Un medico con ese documento ya existe.");
        }

        return MapToResponseMedicDto(medic);
    }

    public async Task<bool> ChangeActiveStatusAsync(int id, DisableMedicDto dto)
    {
        if (dto == null) throw new BadRequestException("Datos invalidos.");

        int affectedRows = await _db.Medics
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.IsActive, dto.IsActive));

        if (affectedRows == 0)
            throw new NotFoundException("El medico no existe.");

        return true;
    }
}
