using System.Collections.Generic;
using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface IWorker
    {
        int CreateWorkerRId(Worker worker);
        void RemoveWorkerById(int id);

        public List<Worker> GetAvailableWorkers(int managerId);
        public bool AssignWorkers(int managerId, int workerId);
        public bool AssingLearned(Worker worker, Topic topic);
        public List<Worker> GetCurrentWorkers(int managerId);

        public List<Topic> GetWorkersTopics(Worker worker);

        public bool RemoveLearned(Worker worker, Topic topic);

        bool AssignGoal(Worker worker, int topicId);

        public List<Topic> GetWorkerGoalsAsTopics(Worker worker);

        public List<Topic> GetAvailableTopics(Worker worker);

    }

}
