using HospitalManagement.Core.Enums;
using System;

namespace HospitalManagement.Core.Entities
{
    public class Appointment : BaseEntity
    {
        public string PatientId { get; set; } = string.Empty;
        public AppUser Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int MedicalServiceId { get; set; }
        public MedicalService MedicalService { get; set; } = null!;

        public DateTime AppointmentDate { get; set; } // Başlangıç saati
        public DateTime EndDate { get; set; }          // Bitiş saati (Başlangıç + Süre)

        public decimal Price { get; set; }             // Randevu anındaki sabit ücret
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending; // Onay durumu
        public string? Notes { get; set; }
    }
}