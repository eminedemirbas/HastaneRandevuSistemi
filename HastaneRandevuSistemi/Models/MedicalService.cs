namespace HastaneRandevuSistemi.Models;

public class MedicalService
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public decimal Price { get; set; }

    public int ClinicId { get; set; }

    public Clinic Clinic { get; set; } = null!;

    public ICollection<Doctor> Doctors { get; set; }
        = new List<Doctor>();

    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();
}