using System.Collections.Generic;

namespace HospitalManagement.Core.Entities
{
    // Çok şubeli hastane/klinik desteği
    public class Hospital : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<HospitalDepartment> HospitalDepartments { get; set; } = new List<HospitalDepartment>();
    }

    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // Kardiyoloji, Göz, Radyoloji vb.
        public string? Description { get; set; }

        public ICollection<HospitalDepartment> HospitalDepartments { get; set; } = new List<HospitalDepartment>();
        public ICollection<MedicalService> Services { get; set; } = new List<MedicalService>();
    }

    // Hastane - Bölüm Many-to-Many ara tablosu
    public class HospitalDepartment : BaseEntity
    {
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}