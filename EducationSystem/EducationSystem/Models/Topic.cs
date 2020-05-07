using System.Collections.Generic;

namespace EducationSystem.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Topic> SubTopics { get; set; }
        public virtual ICollection<WorkerTopic> WorkerTopics { get; set; }
    }
}
