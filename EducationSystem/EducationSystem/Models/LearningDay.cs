using System.Collections.Generic;

namespace EducationSystem.Models
{
    public class LearningDay
    {
        public int Id { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public string Comment { get; set; }
    }
}
