﻿using Microsoft.AspNetCore.Identity;

namespace Gymbokning.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
    }
}
