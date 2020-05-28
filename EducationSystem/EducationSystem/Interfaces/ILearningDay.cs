using EducationSystem.Models;
using System.Collections.Generic;

namespace EducationSystem.Interfaces
{
    public interface ILearningDay
    {
        public List<LearningDay> GetAllLearningDays();
        public List<LearningDay> GetLearningDaysByWorker(Worker worker);
        public void SendMail(LearningDay learningDay);
    }
}
