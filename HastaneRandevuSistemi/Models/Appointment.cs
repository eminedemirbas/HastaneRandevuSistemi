using System.ComponentModel.DataAnnotations;

public class Appointment
{
    public int Id { get; set; }
    
    [Required]
    public string PatientId { get; set; } // Hastanın Identity User ID'si
    
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    
    public DateTime AppointmentDate { get; set; } // Randevu Tarihi ve Saati
    public int DurationMinutes { get; set; } // Hizmet Süresi
    public decimal TotalPrice { get; set; } // Ücret
    
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending; // Onay Durumu
}

public enum AppointmentStatus
{
    Pending,   // Bekliyor
    Approved,  // Onaylandı
    Cancelled  // İptal Edildi
}