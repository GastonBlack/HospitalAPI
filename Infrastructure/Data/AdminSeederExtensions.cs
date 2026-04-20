using HospitalAPI.Features.Admins.Models;
using HospitalAPI.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HospitalAPI.Infrastructure.Data;

public static class AdminSeederExtensions
{
    public static async Task SeedAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
        var adminSeedOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;
        var pendingMigrations = await db.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
            return;

        if (await db.Admins.AnyAsync())
            return;

        if (string.IsNullOrWhiteSpace(adminSeedOptions.Name) ||
            string.IsNullOrWhiteSpace(adminSeedOptions.LastName) ||
            string.IsNullOrWhiteSpace(adminSeedOptions.Document) ||
            string.IsNullOrWhiteSpace(adminSeedOptions.Password))
        {
            throw new InvalidOperationException("La configuracion de AdminSeed es obligatoria.");
        }

        Admin admin = new()
        {
            Name = adminSeedOptions.Name.Trim().ToLower(),
            LastName = adminSeedOptions.LastName.Trim().ToLower(),
            Document = adminSeedOptions.Document.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminSeedOptions.Password),
            Role = AuthRoles.Admin,
            IsActive = true
        };

        await db.Admins.AddAsync(admin);
        await db.SaveChangesAsync();
    }
}
