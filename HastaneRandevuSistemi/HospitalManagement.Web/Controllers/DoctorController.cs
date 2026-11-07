using HospitalManagement.Core.Entities;
using HospitalManagement.Core.Enums;
using HospitalManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalManagement.Web.Controllers
{
    [Authorize(Roles = "Doctor,Admin")] // Doktorlar ve Adminler erişebilir
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DoctorController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Doktorun Randevu Takvimi ve Listesi
        public async Task<IActionResult> Appointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Giriş yapan doktorun profilini bul
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            // Eğer Admin giriş yaptıysa veya doktor henüz profil olarak bağlanmadıysa ilk doktoru ya da tümünü görebilir
            int? doctorId = doctor?.Id;

            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.MedicalService)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .AsQueryable();

            if (doctorId.HasValue)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            ViewBag.DoctorName = doctor != null ? $"{doctor.Title} {User.Identity?.Name}" : "Doktor";

            return View(appointments);
        }

        // Randevu Durumunu Güncelleme (Onaylama, İptal, Tamamlama)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int appointmentId, AppointmentStatus newStatus)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Randevu durumu başarıyla '{newStatus}' olarak güncellendi.";
            return RedirectToAction(nameof(Appointments));
        }
    }
}