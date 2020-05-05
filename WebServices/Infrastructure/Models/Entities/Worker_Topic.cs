using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models.Entities
{
    public class Worker_Topic
    {
        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }


    }
}
