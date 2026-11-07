using HospitalManagement.Core.Enums;
using HospitalManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Data.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date, int serviceDurationMinutes)
        {
            var dayOfWeek = date.DayOfWeek;

            // 1. Doktorun o günkü mesaisini çek
            var workingHour = await _context.DoctorWorkingHours
                .FirstOrDefaultAsync(w => w.DoctorId == doctorId && w.DayOfWeek == dayOfWeek);

            if (workingHour == null)
                return new List<DateTime>(); // Doktor o gün çalışmıyor

            // 2. Doktorun o günkü mevcut randevularını çek
            var existingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId 
                         && a.AppointmentDate.Date == date.Date 
                         && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();

            var availableSlots = new List<DateTime>();

            // 3. Mesai başlangıç ve bitiş saatlerini DateTime'a çevir
            var currentSlotStart = date.Date.Add(workingHour.StartTime);
            var shiftEnd = date.Date.Add(workingHour.EndTime);

            // 4. Slot aralıklarını dolaşarak çakışmayanları listeye ekle
            while (currentSlotStart.AddMinutes(serviceDurationMinutes) <= shiftEnd)
            {
                var currentSlotEnd = currentSlotStart.AddMinutes(serviceDurationMinutes);

                // Çakışma kontrolü: Başlangıç veya bitişi başka bir randevunun aralığına giriyor mu?
                bool isOccupied = existingAppointments.Any(a =>
                    (currentSlotStart >= a.AppointmentDate && currentSlotStart < a.EndDate) ||
                    (currentSlotEnd > a.AppointmentDate && currentSlotEnd <= a.EndDate) ||
                    (currentSlotStart <= a.AppointmentDate && currentSlotEnd >= a.EndDate)
                );

                // Eğer slot geçmişte değilse ve dolu değilse müsait olarak ekle
                if (!isOccupied && currentSlotStart > DateTime.Now)
                {
                    availableSlots.Add(currentSlotStart);
                }

                // Bir sonraki adıma doktorun slot periyodu kadar ilerle (varsayılan 15 dk)
                currentSlotStart = currentSlotStart.AddMinutes(workingHour.SlotDurationMinutes);
            }

            return availableSlots;
        }

        public async Task<bool> HasConflictAsync(int doctorId, DateTime startDateTime, DateTime endDateTime)
        {
            // Belirtilen zaman aralığında iptal edilmemiş başka bir randevu var mı?
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.Status != AppointmentStatus.Cancelled &&
                ((startDateTime >= a.AppointmentDate && startDateTime < a.EndDate) ||
                 (endDateTime > a.AppointmentDate && endDateTime <= a.EndDate) ||
                 (startDateTime <= a.AppointmentDate && endDateTime >= a.EndDate))
            );
        }
    }
}