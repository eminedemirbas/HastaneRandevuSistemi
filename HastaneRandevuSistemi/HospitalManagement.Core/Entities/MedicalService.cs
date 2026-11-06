namespace HospitalManagement.Core.Entities
{
    public class MedicalService : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // Genel Muayene, Diş Dolgusu, MR vb.
        public int DurationInMinutes { get; set; }        // Dakika cinsinden hizmet süresi
        public decimal Price { get; set; }                // Ücret

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}