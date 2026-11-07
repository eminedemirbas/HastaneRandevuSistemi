using HospitalManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // 1. Rolleri Tanımla
            string[] roles = { "Admin", "Doctor", "Patient" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole { Name = role });
                }
            }

            // 2. Varsayılan Admin Kullanıcısı
            string adminEmail = "admin@hospital.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Sistem",
                    LastName = "Yöneticisi",
                    NationalId = "11111111111",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Örnek Hastane ve Bölümler
            if (!context.Hospitals.Any())
            {
                var hospital1 = new Hospital
                {
                    Name = "Şehir Merkez Hastanesi",
                    Address = "Atatürk Bulvarı No:10",
                    City = "Sakarya",
                    PhoneNumber = "0264 111 22 33"
                };

                var hospital2 = new Hospital
                {
                    Name = "Üniversite Araştırma Hastanesi",
                    Address = "Kampüs Yolu No:5",
                    City = "Sakarya",
                    PhoneNumber = "0264 444 55 66"
                };

                await context.Hospitals.AddRangeAsync(hospital1, hospital2);
                await context.SaveChangesAsync();

                var depCardiology = new Department { Name = "Kardiyoloji", Description = "Kalp ve Damar Hastalıkları" };
                var depEye = new Department { Name = "Göz Hastalıkları", Description = "Göz Sağlığı ve Cerrahisi" };
                var depInternal = new Department { Name = "Dahiliye", Description = "İç Hastalıkları" };

                await context.Departments.AddRangeAsync(depCardiology, depEye, depInternal);
                await context.SaveChangesAsync();

                // Hizmetler (Süre ve Ücret tanımlaması - Hoca şartı)
                var service1 = new MedicalService { Name = "Rutin Muayene", DurationInMinutes = 15, Price = 500, DepartmentId = depCardiology.Id };
                var service2 = new MedicalService { Name = "EKO / Kalp Ultrasonu", DurationInMinutes = 30, Price = 1200, DepartmentId = depCardiology.Id };
                var service3 = new MedicalService { Name = "Göz Numarası Ölçümü & Muayene", DurationInMinutes = 20, Price = 600, DepartmentId = depEye.Id };
                var service4 = new MedicalService { Name = "Genel Dahiliye Kontrolü", DurationInMinutes = 15, Price = 450, DepartmentId = depInternal.Id };

                await context.MedicalServices.AddRangeAsync(service1, service2, service3, service4);

                // Şubeler ile Bölümleri Eşle (Çok şubeli yapı)
                context.HospitalDepartments.AddRange(
                    new HospitalDepartment { HospitalId = hospital1.Id, DepartmentId = depCardiology.Id },
                    new HospitalDepartment { HospitalId = hospital1.Id, DepartmentId = depInternal.Id },
                    new HospitalDepartment { HospitalId = hospital2.Id, DepartmentId = depCardiology.Id },
                    new HospitalDepartment { HospitalId = hospital2.Id, DepartmentId = depEye.Id },
                    new HospitalDepartment { HospitalId = hospital2.Id, DepartmentId = depInternal.Id }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}