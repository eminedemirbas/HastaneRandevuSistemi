using HospitalManagement.Core.Entities;
using HospitalManagement.Core.Enums;
using HospitalManagement.Core.Interfaces;
using HospitalManagement.Data;
using HospitalManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalManagement.Web.Controllers
{
    [Authorize] // Randevu için oturum açmış olmak şarttır
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(AppDbContext context, IAppointmentService appointmentService, UserManager<AppUser> userManager)
        {
            _context = context;
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        // Hastanın Kendi Randevularını Listelediği Ekran
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.HospitalDepartment).ThenInclude(hd => hd.Hospital)
                .Include(a => a.Doctor).ThenInclude(d => d.HospitalDepartment).ThenInclude(hd => hd.Department)
                .Include(a => a.MedicalService)
                .Where(a => a.PatientId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // GET: /Appointment/Create (Randevu Alma Formu)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Hospitals = new SelectList(await _context.Hospitals.ToListAsync(), "Id", "Name");
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            return View(new AppointmentCreateViewModel());
        }

        // AJAX: Seçilen Hastane ve Bölüme göre Doktorları getir
        [HttpGet]
        public async Task<JsonResult> GetDoctors(int hospitalId, int departmentId)
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.HospitalDepartment.HospitalId == hospitalId && d.HospitalDepartment.DepartmentId == departmentId)
                .Select(d => new { id = d.Id, fullName = $"{d.Title} {d.User.FullName} ({d.Specialty})" })
                .ToListAsync();

            return Json(doctors);
        }

        // AJAX: Seçilen Bölüme göre Hizmetleri getir
        [HttpGet]
        public async Task<JsonResult> GetServices(int departmentId)
        {
            var services = await _context.MedicalServices
                .Where(s => s.DepartmentId == departmentId)
                .Select(s => new { id = s.Id, name = $"{s.Name} - {s.DurationInMinutes} Dk ({s.Price:C2})" })
                .ToListAsync();

            return Json(services);
        }

        // AJAX: Müsait Saatleri (Slotları) Getir
        [HttpGet]
        public async Task<JsonResult> GetAvailableSlots(int doctorId, int serviceId, string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return Json(new List<string>());

            var service = await _context.MedicalServices.FindAsync(serviceId);
            if (service == null) return Json(new List<string>());

            var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, parsedDate, service.DurationInMinutes);

            var result = slots.Select(s => new {
                value = s.ToString("yyyy-MM-ddTHH:mm:ss"),
                display = s.ToString("HH:mm")
            });

            return Json(result);
        }

        // POST: Randevu Kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            var service = await _context.MedicalServices.FindAsync(model.MedicalServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Seçilen sağlık hizmeti bulunamadı.");
                return View(model);
            }

            var startDateTime = model.SelectedTime;
            var endDateTime = startDateTime.AddMinutes(service.DurationInMinutes);

            // Çakışma kontrolü
            bool hasConflict = await _appointmentService.HasConflictAsync(model.DoctorId, startDateTime, endDateTime);
            if (hasConflict)
            {
                ModelState.AddModelError("", "Seçilen saat az önce doldu veya başka bir randevuyla çakışıyor. Lütfen başka bir saat seçiniz.");
                ViewBag.Hospitals = new SelectList(await _context.Hospitals.ToListAsync(), "Id", "Name");
                ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointment = new Appointment
            {
                PatientId = userId!,
                DoctorId = model.DoctorId,
                MedicalServiceId = model.MedicalServiceId,
                AppointmentDate = startDateTime,
                EndDate = endDateTime,
                Price = service.Price, // O anki fiyat sabitlenir
                Status = AppointmentStatus.Pending, // Onay bekliyor
                Notes = model.Notes
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu ve onay bekliyor.";
            return RedirectToAction(nameof(MyAppointments));
        }

        // Randevu İptal Et
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.PatientId == userId);

            if (appointment != null && appointment.Status != AppointmentStatus.Completed)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevunuz iptal edildi.";
            }

            return RedirectToAction(nameof(MyAppointments));
        }
    }
}