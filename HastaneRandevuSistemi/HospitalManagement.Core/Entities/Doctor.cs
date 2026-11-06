using System;
using System.Collections.Generic;

namespace HospitalManagement.Core.Entities
{
    public class Doctor : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;

        public string Specialty { get; set; } = string.Empty;
        public string? Title { get; set; } // Uzm. Dr., Prof. Dr.

        public int HospitalDepartmentId { get; set; }
        public HospitalDepartment HospitalDepartment { get; set; } = null!;

        public ICollection<DoctorWorkingHour> WorkingHours { get; set; } = new List<DoctorWorkingHour>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

    public class DoctorWorkingHour : BaseEntity
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 15;
    }
}