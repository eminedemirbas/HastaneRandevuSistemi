namespace HospitalManagement.Core.Enums
{
    public enum AppointmentStatus
    {
        Pending = 1,   // Onay Bekliyor
        Approved = 2,  // Onaylandı
        Cancelled = 3, // İptal Edildi
        Completed = 4  // Tamamlandı
    }
}