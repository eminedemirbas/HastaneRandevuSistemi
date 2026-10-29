namespace HastaneRandevuSistemi.Models;

public class Patient : User
{
    public string PhoneNumber { get; set; } = string.Empty;

    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();
}