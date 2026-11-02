public class Doctor
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; } 
    [Required, MaxLength(100)]
    public string FullName { get; set; }
    
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }
    
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    
    public ICollection<DoctorService> DoctorServices { get; set; }
    public ICollection<DoctorAvailability> Availabilities { get; set; }
    public ICollection<Appointment> Appointments { get; set; }
}