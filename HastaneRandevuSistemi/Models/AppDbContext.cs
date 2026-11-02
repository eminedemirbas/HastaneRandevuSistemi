using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<DoctorService> DoctorServices { get; set; }
    public DbSet<DoctorAvailability> Availabilities { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<DoctorService>()
            .HasKey(ds => new { ds.DoctorId, ds.ServiceId });
    }
}

public class DoctorService
{
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}