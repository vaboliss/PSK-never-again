using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationSystem.Models
{
    public class TopicModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public bool Learned { get;set; }
        public bool GoalsLearned { get; set; }
    }
}
