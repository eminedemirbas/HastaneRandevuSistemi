using HospitalManagement.Core.Entities;
using HospitalManagement.Data;
using HospitalManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Ana Admin Paneli
        public async Task<IActionResult> Index()
        {
            ViewBag.HospitalCount = await _context.Hospitals.CountAsync();
            ViewBag.DoctorCount = await _context.Doctors.CountAsync();
            ViewBag.DepartmentCount = await _context.Departments.CountAsync();
            ViewBag.AppointmentCount = await _context.Appointments.CountAsync();
            return View();
        }

        #region 1. HASTANE YÖNETİMİ
        public async Task<IActionResult> Hospitals()
        {
            var hospitals = await _context.Hospitals.ToListAsync();
            return View(hospitals);
        }

        [HttpGet]
        public IActionResult CreateHospital() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHospital(Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                _context.Hospitals.Add(hospital);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Hospitals));
            }
            return View(hospital);
        }
        #endregion

        #region 2. BÖLÜM & HİZMET YÖNETİMİ
        public async Task<IActionResult> Departments()
        {
            var depts = await _context.Departments.Include(d => d.Services).ToListAsync();
            return View(depts);
        }

        [HttpGet]
        public IActionResult CreateDepartment() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Departments));
            }
            return View(department);
        }

        [HttpGet]
        public async Task<IActionResult> CreateService(int departmentId)
        {
            var dept = await _context.Departments.FindAsync(departmentId);
            if (dept == null) return NotFound();

            var model = new MedicalService { DepartmentId = departmentId };
            ViewBag.DepartmentName = dept.Name;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(MedicalService service)
        {
            if (ModelState.IsValid)
            {
                _context.MedicalServices.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Departments));
            }
            return View(service);
        }
        #endregion

        #region 3. DOKTOR VE ÇALIŞMA SAATLERİ YÖNETİMİ
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.HospitalDepartment).ThenInclude(hd => hd.Hospital)
                .Include(d => d.HospitalDepartment).ThenInclude(hd => hd.Department)
                .Include(d => d.WorkingHours)
                .ToListAsync();

            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDoctor()
        {
            ViewBag.Hospitals = new SelectList(await _context.Hospitals.ToListAsync(), "Id", "Name");
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(DoctorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. AppUser oluştur
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true
                };

                var userResult = await _userManager.CreateAsync(user, model.Password);
                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    ViewBag.Hospitals = new SelectList(await _context.Hospitals.ToListAsync(), "Id", "Name");
                    ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
                    return View(model);
                }

                await _userManager.AddToRoleAsync(user, "Doctor");

                // 2. Hastane-Bölüm ilişkisi var mı kontrol et, yoksa bağla
                var hospDept = await _context.HospitalDepartments
                    .FirstOrDefaultAsync(hd => hd.HospitalId == model.HospitalId && hd.DepartmentId == model.DepartmentId);

                if (hospDept == null)
                {
                    hospDept = new HospitalDepartment
                    {
                        HospitalId = model.HospitalId,
                        DepartmentId = model.DepartmentId
                    };
                    _context.HospitalDepartments.Add(hospDept);
                    await _context.SaveChangesAsync();
                }

                // 3. Doktor profilini oluştur
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Specialty = model.Specialty,
                    Title = model.Title,
                    HospitalDepartmentId = hospDept.Id
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Doctors));
            }

            ViewBag.Hospitals = new SelectList(await _context.Hospitals.ToListAsync(), "Id", "Name");
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            return View(model);
        }

        // Doktora Mesai Saati Ekleme
        [HttpGet]
        public async Task<IActionResult> AddWorkingHour(int doctorId)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == doctorId);
            if (doctor == null) return NotFound();

            var model = new WorkingHourViewModel
            {
                DoctorId = doctor.Id,
                DoctorName = $"{doctor.Title} {doctor.User.FullName}"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWorkingHour(WorkingHourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var workingHour = new DoctorWorkingHour
                {
                    DoctorId = model.DoctorId,
                    DayOfWeek = model.DayOfWeek,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    SlotDurationMinutes = model.SlotDurationMinutes
                };

                _context.DoctorWorkingHours.Add(workingHour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Doctors));
            }
            return View(model);
        }
        #endregion
    }
}