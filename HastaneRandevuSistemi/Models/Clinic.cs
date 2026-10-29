namespace HastaneRandevuSistemi.Models;

public class Clinic
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TimeSpan OpeningTime { get; set; }

    public TimeSpan ClosingTime { get; set; }

    public int HospitalId { get; set; }

    public Hospital Hospital { get; set; } = null!;

    public ICollection<Doctor> Doctors { get; set; }
        = new List<Doctor>();

    public ICollection<MedicalService> Services { get; set; }
        = new List<MedicalService>();
}