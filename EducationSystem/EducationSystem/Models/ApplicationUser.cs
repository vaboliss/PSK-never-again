using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Models
{
    public class ApplicationUser: IdentityUser
    {
        public int WorkerId { get; set; }
    }
}
