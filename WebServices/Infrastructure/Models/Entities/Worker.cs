using System.Collections.Generic;

namespace Infrastructure.Models.Entities
{
    public class Worker
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Restriction Restriction { get; set; }
        public List<Worker> Subordinates { get; set; }
        public List<Topic> LearntTopics { get; set; }
    }
}