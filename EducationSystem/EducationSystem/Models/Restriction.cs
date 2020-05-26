using System.Text.Json.Serialization;

namespace EducationSystem.Models
{
    public class Restriction
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int WorkerId { get; set; }
        [JsonIgnore]
        public Worker Worker { get; set; }
        public int MaxConsecutiveDays { get; set; }
        public int MaxPerMonth { get; set; }
        public int MaxPerYear { get; set; }
        public int MaxPerQuarter { get; set; }
    }
}
