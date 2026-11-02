using System.ComponentModel.DataAnnotations;

public class Clinic
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } // Klinik Adı
    public string WorkingHours { get; set; } // Çalışma Saatleri
    
    public ICollection<Department> Departments { get; set; }
    public ICollection<Doctor> Doctors { get; set; }
}

public class Department
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } // Bölüm Adı
    
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }
    public ICollection<Doctor> Doctors { get; set; }
}