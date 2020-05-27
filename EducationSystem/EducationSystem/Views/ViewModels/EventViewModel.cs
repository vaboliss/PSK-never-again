using System;

namespace EducationSystem.Views.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public string TopicName { get; set; }
        public string TopicDescription { get; set; }
        public string Comments { get; set; }
        public int WorkerId { get; set; }
    }
}
