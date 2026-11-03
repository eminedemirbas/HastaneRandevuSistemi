using Microsoft.EntityFrameworkCore;

public class AppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> CreateAppointmentAsync(string patientId, int doctorId, int serviceId, DateTime requestedDateTime)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Availabilities)
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        var service = await _context.Services.FindAsync(serviceId);

        if (doctor == null || service == null)
            return (false, "Doktor veya hizmet bulunamadı.");

        var requestedDayOfWeek = requestedDateTime.DayOfWeek;
        var requestedStartTime = requestedDateTime.TimeOfDay;
        var requestedEndTime = requestedStartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));

        var availability = doctor.Availabilities
            .FirstOrDefault(a => a.DayOfWeek == requestedDayOfWeek &&
                                 requestedStartTime >= a.StartTime &&
                                 requestedEndTime <= a.EndTime);

        if (availability == null)
            return (false, "Seçilen saat doktorun çalışma saatleri dışındadır veya doktor o gün çalışmamaktadır.");

        var existingAppointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId && 
                        a.Status != AppointmentStatus.Cancelled &&
                        a.AppointmentDate.Date == requestedDateTime.Date)
            .ToListAsync();

        bool hasConflict = existingAppointments.Any(a => 
        {
            var existingStart = a.AppointmentDate.TimeOfDay;
            var existingEnd = existingStart.Add(TimeSpan.FromMinutes(a.DurationMinutes));

            return requestedStartTime < existingEnd && requestedEndTime > existingStart;
        });

        if (hasConflict)
            return (false, "Seçilen zaman diliminde doktorun başka bir randevusu bulunmaktadır. Lütfen başka bir saat seçiniz.");

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            ServiceId = serviceId,
            AppointmentDate = requestedDateTime,
            DurationMinutes = service.DurationMinutes,
            TotalPrice = service.Price,
            Status = AppointmentStatus.Pending
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return (true, "Randevunuz başarıyla oluşturuldu, onay bekleniyor.");
    }
}