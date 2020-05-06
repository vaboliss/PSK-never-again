using System.Collections.Generic;

namespace Infrastructure.Models.Entities
{
    public class LearningDay
    {
        public int Id { get; set; }
        public List<Topic> Topics { get; set; }
        public string Comment { get; set; }
    }
}
