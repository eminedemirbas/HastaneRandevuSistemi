using Microsoft.AspNetCore.Identity;
using System;

namespace HospitalManagement.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        // Kapsülleme
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public class AppRole : IdentityRole
    {
    }
}