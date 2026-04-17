using Microsoft.EntityFrameworkCore;
using HospitalAPI.Features.Tickets.Models;
using HospitalAPI.Features.Medics.Models;
using HospitalAPI.Features.Patients.Models;

namespace HospitalAPI.Infrastructure.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Medic> Medics { get; set; }
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /// ///////////////////////////////
        /// Patient Restrictions.
        /// ///////////////////////////////
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(p => p.LastName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(p => p.Document)
                .HasMaxLength(8) // Uruguay.
                .IsRequired();

            entity.Property(p => p.PasswordHash)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(p => p.Role)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(p => p.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            entity.HasIndex(p => p.Document)
                .IsUnique();

            entity.Ignore(p => p.FullName);
        });

        /// ///////////////////////////////
        /// Medic Restrictions.
        /// ///////////////////////////////
        modelBuilder.Entity<Medic>(entity =>
        {
            entity.Property(m => m.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(m => m.LastName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(m => m.Document)
                .HasMaxLength(8) // Uruguay.
                .IsRequired();

            entity.Property(m => m.PasswordHash)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(m => m.Role)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(m => m.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            entity.HasIndex(p => p.Document)
                .IsUnique();

            entity.Property(m => m.Specialty)
                .HasMaxLength(50)
                .IsRequired();

            entity.Ignore(m => m.FullName);
        });

        /// ///////////////////////////////
        /// Ticket Restrictions.
        /// ///////////////////////////////
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(t => t.AppointmentDate)
                .IsRequired();

            entity.Property(t => t.CreationDate)
                .IsRequired();

            entity.Property(t => t.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(t => t.Medic)
                .WithMany(m => m.Tickets)
                .HasForeignKey(t => t.MedicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Patient)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
