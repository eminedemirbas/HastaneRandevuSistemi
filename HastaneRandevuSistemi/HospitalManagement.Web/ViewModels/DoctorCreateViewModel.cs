using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Web.ViewModels
{
    public class DoctorCreateViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [Display(Name = "Geçici Şifre")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [Display(Name = "Uzmanlık Alanı")]
        public string Specialty { get; set; } = string.Empty;

        [Display(Name = "Unvan (Örn: Doç. Dr.)")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Hastane seçilmelidir.")]
        [Display(Name = "Hastane")]
        public int HospitalId { get; set; }

        [Required(ErrorMessage = "Bölüm seçilmelidir.")]
        [Display(Name = "Bölüm")]
        public int DepartmentId { get; set; }
    }
}