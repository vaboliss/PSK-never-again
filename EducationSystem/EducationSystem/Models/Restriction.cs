using System;

namespace EducationSystem.Models
{
    public class Restriction
    {
        public int Id { get; set; }
        public int DayCap { get; set; }
        public DateTime Expires { get; set; }
    }
}
