using System;

namespace Infrastructure.Models.Entities
{
    public class Restriction
    {
        public int Id { get; set; }
        public int DayCap { get; set; }
        public DateTime Expires { get; set; }
    }
}
