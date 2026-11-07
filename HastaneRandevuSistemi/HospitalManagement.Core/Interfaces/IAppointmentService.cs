using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Core.Interfaces
{
    public interface IAppointmentService
    {
        // Belirli bir doktor, tarih ve işlem süresine göre müsait saatleri döner
        Task<List<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date, int serviceDurationMinutes);

        // Randevu saati başka bir randevuyla veya mesai dışıyla çakışıyor mu kontrol eder
        Task<bool> HasConflictAsync(int doctorId, DateTime startDateTime, DateTime endDateTime);
    }
}