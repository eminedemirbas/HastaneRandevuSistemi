using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Web.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Lütfen bir hastane seçiniz.")]
        [Display(Name = "Hastane")]
        public int HospitalId { get; set; }

        [Required(ErrorMessage = "Lütfen bir bölüm seçiniz.")]
        [Display(Name = "Bölüm")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Lütfen bir işlem / sağlık hizmeti seçiniz.")]
        [Display(Name = "Yapılacak İşlem / Hizmet")]
        public int MedicalServiceId { get; set; }

        [Required(ErrorMessage = "Lütfen doktor seçiniz.")]
        [Display(Name = "Doktor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Lütfen randevu tarihi seçiniz.")]
        [DataType(DataType.Date)]
        [Display(Name = "Randevu Tarihi")]
        public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Lütfen müsait bir saat seçiniz.")]
        [Display(Name = "Randevu Saati")]
        public DateTime SelectedTime { get; set; }

        [Display(Name = "Şikayet / Not")]
        public string? Notes { get; set; }
    }
}