namespace Infrastructure.Models.Entities
{
    public class Goal
    {
        public int Id { get; set; }
        public Topic Topic { get; set; }
        public Worker Worker { get; set; }
    }
}
