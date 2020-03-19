using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models.Entities
{
    public class Topic
    {
        public int SubTopic_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Topic> SubTopics { get; set; }
    }
}
