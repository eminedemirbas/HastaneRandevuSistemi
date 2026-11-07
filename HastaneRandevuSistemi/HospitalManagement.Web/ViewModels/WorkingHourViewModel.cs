using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Web.ViewModels
{
    public class WorkingHourViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Gün")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0);

        [Required]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(17, 0, 0);

        [Required]
        [Display(Name = "Randevu Aralığı (Dakika)")]
        public int SlotDurationMinutes { get; set; } = 15;
    }
}