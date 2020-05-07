using System.Collections.Generic;

namespace EducationSystem.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Restriction Restriction { get; set; }
        public virtual ICollection<Worker> Subordinates { get; set; }
        public virtual ICollection<WorkerTopic> WorkerTopics { get; set; } 
    }
}