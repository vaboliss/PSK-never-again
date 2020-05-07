using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Models
{
    public class WorkerTopic
    {
        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
