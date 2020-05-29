using EducationSystem.Static;
using System.Collections.Generic;

namespace EducationSystem.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Restriction Restriction { get; private set; }

        public Team TeamManager { get; set; }
        public virtual ICollection<Worker> Subordinates { get; set; }
        public virtual ICollection<WorkerTopic> WorkerTopics { get; set; }
        public virtual ICollection<Goal> WorkerGoals { get; set; }
        public virtual Worker Parent { get; set; }
        public void SetMaxConsecutiveDays(int value)
        {
            if (value >= 0 && value <= GlobalRestrictions.MaxPerQuarter)
                Restriction.MaxConsecutiveDays = value;
        }
        public void SetMaxPerYear(int value)
        {
            if (value >= 0 && value <= GlobalRestrictions.MaxPerQuarter * 4)
                Restriction.MaxPerYear = value;
        }
        public void SetMaxPerMonth(int value)
        {
            if (value >= 0 && value <= GlobalRestrictions.MaxPerQuarter)
                Restriction.MaxPerMonth = value;
        }
        public void SetMaxPerQuarter(int value)
        {
            if (value >= 0 && value <= GlobalRestrictions.MaxPerQuarter)
                Restriction.MaxPerQuarter = value;
        }
    }
}