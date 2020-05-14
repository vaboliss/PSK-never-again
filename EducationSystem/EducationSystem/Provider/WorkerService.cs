using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;

namespace EducationSystem.Provider
{
    public class WorkerService : IWorker
    {
        private readonly EducationSystemDbContext _edu;
        private readonly ITopic _topicService;

        public WorkerService(EducationSystemDbContext edu, ITopic topicService)
        {
            _edu = edu;
            _topicService = topicService;
        }

        // Assigns goal to worker, topicId needed
        public bool AssignGoal (Worker worker, int topicId)
        {
            Topic topic = _topicService.GetTopicById(topicId);
            if (topic is null)
                return false;
            Goal goalToCreate = new Goal();
            goalToCreate.Worker = worker;
            goalToCreate.Topic = topic;
            try
            {
                _edu.Add(goalToCreate);
            }
            catch
            {
                return false;
            }
            _edu.SaveChanges();
            return true;
        }
    }
}
