using Newtonsoft.Json;
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
        [JsonIgnore]
        public Topic Parent { get; set; }
        [JsonIgnore]
        public ICollection<Topic> SubTopics { get; set; } = new List<Topic>();
        [JsonIgnore]
        public virtual ICollection<WorkerTopic> WorkerTopics { get; set; }
    }
}
