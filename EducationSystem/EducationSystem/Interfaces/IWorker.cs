using System.Collections.Generic;
using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface IWorker
    {
        bool AssignGoal(Worker worker, int topicId);

        public List<Topic> GetWorkerGoalsAsTopics(Worker worker);

        public List<Topic> GetAvailableTopics(Worker worker);
    }

}
