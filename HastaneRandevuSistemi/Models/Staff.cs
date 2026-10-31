namespace HastaneRandevuSistemi.Models;

public class Staff : User
{
    public string Position { get; set; } = string.Empty;

    public int ClinicId { get; set; }

    public Clinic Clinic { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }
}