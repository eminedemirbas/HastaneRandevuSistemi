using HastaneRandevuSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevuSistemi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<MedicalService> MedicalServices => Set<MedicalService>();
    public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Kullanıcı türleri için kalıtım (Inheritance)
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Patient>("Patient")
            .HasValue<Doctor>("Doctor")
            .HasValue<Staff>("Staff")
            .HasValue<Administrator>("Administrator");

        // Ücret alanlarının hassasiyet ayarları
        modelBuilder.Entity<MedicalService>()
            .Property(s => s.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Price)
            .HasPrecision(10, 2);

        // Doktor ve sağlık hizmetleri arasındaki çoktan çoğa ilişki
        modelBuilder.Entity<Doctor>()
            .HasMany(d => d.Services)
            .WithMany(s => s.Doctors);

        // Klinik ve bölüm ilişkisi
        modelBuilder.Entity<Clinic>()
            .HasMany(c => c.Departments)
            .WithOne(d => d.Clinic)
            .HasForeignKey(d => d.ClinicId);

        // Doktor ve bölüm ilişkisi
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Department)
            .WithMany(d => d.Doctors)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Hasta ve randevu ilişkisi
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Doktor ve randevu ilişkisi
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}