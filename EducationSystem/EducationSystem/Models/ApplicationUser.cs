using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Models
{
    public class ApplicationUser: IdentityUser
    {
        public int workerId { get; set; }
    }
}
