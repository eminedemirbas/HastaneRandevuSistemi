namespace HastaneRandevuSistemi.Models;

public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ClinicId { get; set; }

    public Clinic Clinic { get; set; } = null!;

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
}