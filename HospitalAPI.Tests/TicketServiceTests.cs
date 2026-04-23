using HospitalAPI.Features.Medics.Models;
using HospitalAPI.Features.Patients.Models;
using HospitalAPI.Features.Tickets.Constants;
using HospitalAPI.Features.Tickets.DTOs;
using HospitalAPI.Features.Tickets.Services;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Tests;

public class TicketServiceTests
{
    private static async Task<SqliteConnection> CreateConnectionAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        return connection;
    }

    private static async Task<HospitalDbContext> CreateDbContextAsync(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new HospitalDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return db;
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBadRequestException_WhenAppointmentDateIsTooSoon()
    {
        await using var connection = await CreateConnectionAsync();
        await using var db = await CreateDbContextAsync(connection);
        var service = new TicketService(db);
        var dto = new CreateTicketDto
        {
            MedicId = 1,
            AppointmentDate = DateTime.UtcNow.AddMinutes(10)
        };

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.CreateAsync(1, dto));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTicket_WhenDataIsValid()
    {
        await using var connection = await CreateConnectionAsync();
        int medicId;
        int patientId;
        await using (var seedDb = await CreateDbContextAsync(connection))
        {
            var medic = new Medic
            {
                Name = "Ana",
                LastName = "Lopez",
                Document = "12345678",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret123"),
                Specialty = "No Specialty"
            };
            var patient = new Patient
            {
                Name = "Juan",
                LastName = "Perez",
                Document = "87654321",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret456")
            };
            seedDb.Medics.Add(medic);
            seedDb.Patients.Add(patient);
            await seedDb.SaveChangesAsync();
            medicId = medic.Id;
            patientId = patient.Id;
        }

        await using var db = await CreateDbContextAsync(connection);
        var service = new TicketService(db);
        var appointmentDate = DateTime.UtcNow.AddHours(2);
        var dto = new CreateTicketDto
        {
            MedicId = medicId,
            AppointmentDate = appointmentDate
        };

        var result = await service.CreateAsync(patientId, dto);
        var ticket = await db.Tickets.SingleAsync();

        Assert.Equal(ticket.Id, result.Id);
        Assert.Equal(medicId, result.MedicId);
        Assert.Equal(patientId, result.PatientId);
        Assert.Equal(TicketStatuses.Pending, result.Status);
        Assert.Equal(appointmentDate, result.AppointmentDate);

        Assert.Equal(medicId, ticket.MedicId);
        Assert.Equal(patientId, ticket.PatientId);
        Assert.Equal(TicketStatuses.Pending, ticket.Status);
    }
}
