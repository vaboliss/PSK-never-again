using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationSystem.Models
{
    public class Topic
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
       // public int? ParentId { get; set; }
      //  [ForeignKey("ParentId")]
        public Topic Parent { get; set; }
        public ICollection<Topic> SubTopics { get; set; } = new List<Topic>();
        public virtual ICollection<WorkerTopic> WorkerTopics { get; set; }
    }
}
