using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public string TeamName { get; set; }
        [Required]
        public int WorkerId { get; set; }
        public virtual Worker Manager { get; set; }
    }
}
