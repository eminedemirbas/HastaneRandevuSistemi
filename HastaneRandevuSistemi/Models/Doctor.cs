namespace HastaneRandevuSistemi.Models;

public class Doctor : User
{
    public string Specialty { get; set; } = string.Empty;

    public int ClinicId { get; set; }

    public Clinic Clinic { get; set; } = null!;

    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();

    public ICollection<DoctorAvailability> AvailabilitySlots { get; set; }
        = new List<DoctorAvailability>();

    public ICollection<MedicalService> Services { get; set; }
        = new List<MedicalService>();
}