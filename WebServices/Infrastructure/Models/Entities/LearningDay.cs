using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models.Entities
{
    public class LearningDay
    {
        public int LearningDay_Id { get; set; }
        public List<Topic> Topics { get; set; }
        public string Commnents { get; set; }
    }
}
