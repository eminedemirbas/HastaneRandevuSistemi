using HospitalManagement.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Hospital> Hospitals => Set<Hospital>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<HospitalDepartment> HospitalDepartments => Set<HospitalDepartment>();
        public DbSet<MedicalService> MedicalServices => Set<MedicalService>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<DoctorWorkingHour> DoctorWorkingHours => Set<DoctorWorkingHour>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ücret alanları için SQL decimal hassasiyet ayarı
            builder.Entity<MedicalService>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Appointment>()
                .Property(a => a.Price)
                .HasColumnType("decimal(18,2)");

            // Çakışmaları ve silme davranışlarını yönetmek için cascade restrict kuralı
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.MedicalService)
                .WithMany()
                .HasForeignKey(a => a.MedicalServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}