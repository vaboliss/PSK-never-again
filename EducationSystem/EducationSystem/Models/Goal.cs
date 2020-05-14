namespace EducationSystem.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }
        public int WorkerId { get; set; }
        public virtual Worker Worker { get; set; }
    }
}
