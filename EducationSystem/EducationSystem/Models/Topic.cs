using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationSystem.Models
{
    public class Topic
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public Topic Parent { get; set; }
        public ICollection<Topic> SubTopics { get; set; } = new List<Topic>();
        public virtual ICollection<WorkerTopic> WorkerTopics { get; set; }
    }
}
