﻿using System;
using System.Collections.Generic;

namespace EducationSystem.Models
{
    public class LearningDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }
        public string Comment { get; set; }
    }
}
