namespace EducationSystem.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public Worker Manager { get; set; }
    }
}
