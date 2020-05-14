namespace EducationSystem.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }

        public int WorkerId { get; set; }
        public virtual Worker Manager { get; set; }
    }
}
