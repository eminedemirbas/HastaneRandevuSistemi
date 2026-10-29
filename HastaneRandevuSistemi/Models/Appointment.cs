namespace HastaneRandevuSistemi.Models;

public enum AppointmentStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled,
    Completed
}

public class Appointment
{
    public int Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal Price { get; set; }

    public AppointmentStatus Status { get; set; }
        = AppointmentStatus.Pending;

    public int PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public int MedicalServiceId { get; set; }

    public MedicalService MedicalService { get; set; } = null!;
}