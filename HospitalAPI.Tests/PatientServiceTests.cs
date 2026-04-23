using HospitalAPI.Features.Patients.DTOs;
using HospitalAPI.Features.Patients.Models;
using HospitalAPI.Features.Patients.Services;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Tests;

public class PatientServiceTests
{
    private static HospitalDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new HospitalDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePatient_WhenDocumentDoesNotExist()
    {
        await using var db = CreateDbContext();
        var service = new PatientService(db);
        var dto = new CreatePatientDto
        {
            Name = " juan ",
            LastName = " perez ",
            Document = "12345678",
            Password = "secret123"
        };

        var result = await service.CreateAsync(dto);

        Assert.Equal("Juan", result.Name);
        Assert.Equal("Perez", result.LastName);
        Assert.Equal("Juan Perez", result.FullName);
        Assert.Equal("12345678", result.Document);
        Assert.True(result.IsActive);
        Assert.Single(db.Patients);
        Assert.True(BCrypt.Net.BCrypt.Verify("secret123", db.Patients.Single().PasswordHash));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenDocumentAlreadyExists()
    {
        await using var db = CreateDbContext();
        db.Patients.Add(new Patient
        {
            Name = "Maria",
            LastName = "Silva",
            Document = "12345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret123")
        });
        await db.SaveChangesAsync();
        var service = new PatientService(db);
        var dto = new CreatePatientDto
        {
            Name = "Juan",
            LastName = "Perez",
            Document = "12345678",
            Password = "secret456"
        };

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBadRequestException_WhenDtoIsNull()
    {
        await using var db = CreateDbContext();
        var service = new PatientService(db);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.CreateAsync(null!));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenPatientDoesNotExist()
    {
        await using var db = CreateDbContext();
        var service = new PatientService(db);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPatient_WhenPatientExists()
    {
        await using var db = CreateDbContext();
        var patient = new Patient
        {
            Name = "Juan",
            LastName = "Perez",
            Document = "12345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret123")
        };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        var service = new PatientService(db);

        var result = await service.GetByIdAsync(patient.Id);

        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
        Assert.Equal("Juan", result.Name);
        Assert.Equal("Perez", result.LastName);
        Assert.Equal("Juan Perez", result.FullName);
        Assert.Equal("12345678", result.Document);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenPatientDoesNotExist()
    {
        await using var db = CreateDbContext();
        var service = new PatientService(db);
        var dto = new UpdatePatientDto
        {
            Name = "Juan",
            LastName = "Perez",
            Document = "12345678",
            Password = "secret123"
        };

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAsync(999, dto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowBadRequestException_WhenDataIsUnchanged()
    {
        await using var db = CreateDbContext();
        var patient = new Patient
        {
            Name = "Juan",
            LastName = "Perez",
            Document = "12345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret123")
        };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        var service = new PatientService(db);
        var dto = new UpdatePatientDto
        {
            Name = "juan",
            LastName = "perez",
            Document = "12345678",
            Password = "secret123"
        };

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.UpdateAsync(patient.Id, dto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowConflictException_WhenDocumentAlreadyExists()
    {
        await using var db = CreateDbContext();
        var existingPatient = new Patient
        {
            Name = "Juan",
            LastName = "Perez",
            Document = "12345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret123")
        };
        var otherPatient = new Patient
        {
            Name = "Maria",
            LastName = "Silva",
            Document = "87654321",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret456")
        };
        db.Patients.AddRange(existingPatient, otherPatient);
        await db.SaveChangesAsync();
        var service = new PatientService(db);
        var dto = new UpdatePatientDto
        {
            Name = "Maria",
            LastName = "Silva",
            Document = "12345678",
            Password = "secret456"
        };

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.UpdateAsync(otherPatient.Id, dto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePatient_WhenDataIsDifferent()
    {
        await using var db = CreateDbContext();
        var patient = new Patient
        {
            Name = "Juan",
            LastName = "Perez",
            Document = "12345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret123")
        };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        var service = new PatientService(db);
        var dto = new UpdatePatientDto
        {
            Name = "carlos",
            LastName = "lopez",
            Document = "87654321",
            Password = "newsecret123"
        };

        var result = await service.UpdateAsync(patient.Id, dto);
        var updatedPatient = await db.Patients.FindAsync(patient.Id);

        Assert.NotNull(result);
        Assert.Equal(patient.Id, result!.Id);
        Assert.Equal("Carlos", result.Name);
        Assert.Equal("Lopez", result.LastName);
        Assert.Equal("Carlos Lopez", result.FullName);
        Assert.Equal("87654321", result.Document);
        Assert.True(result.IsActive);

        Assert.NotNull(updatedPatient);
        Assert.Equal("Carlos", updatedPatient!.Name);
        Assert.Equal("Lopez", updatedPatient.LastName);
        Assert.Equal("87654321", updatedPatient.Document);
        Assert.True(BCrypt.Net.BCrypt.Verify("newsecret123", updatedPatient.PasswordHash));
    }
}
