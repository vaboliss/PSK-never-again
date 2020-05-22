using System;

namespace EducationSystem.Models
{
    public class Restriction
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
        public int MaxConsecutiveDays { get; set; }
        public int MaxPerMonth { get; set; }
        public int MaxPerYear { get; set; }
    }
}
