using System.Collections.Generic;
using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface IWorker
    {

        public List<Worker> GetAvailableWorkers(int managerId);
        public bool  AssignWorkers(int managerId, int workerId);
        public List<Worker> GetCurrentWorkers(int managerId);

        bool AssignGoal(Worker worker, int topicId);

        public List<Topic> GetWorkerGoalsAsTopics(Worker worker);

        public List<Topic> GetAvailableTopics(Worker worker);

    }

}
