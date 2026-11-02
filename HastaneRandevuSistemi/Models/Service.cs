using System.ComponentModel.DataAnnotations;

public class Service
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } // Hizmet Adı 
    public int DurationMinutes { get; set; } // İşlem Süresi 
    public decimal Price { get; set; } // Ücret
    
    public ICollection<DoctorService> DoctorServices { get; set; }
}