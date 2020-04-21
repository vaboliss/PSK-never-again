using System.Collections.Generic;

namespace Infrastructure.Models.Entities
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Topic> SubTopics { get; set; }
    }
}
