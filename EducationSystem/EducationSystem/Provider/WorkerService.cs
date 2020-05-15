using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;
using System.Collections.Generic;
using System.Linq;

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

        // Returns a list of topics that are assigned as goals to the particular worker
        public List<Topic> GetWorkerGoalsAsTopics (Worker worker)
        {
            var databaseResults = _edu.Goals.Where(g => g.WorkerId == worker.Id);
            if (databaseResults.Any())
            {
                var topics = databaseResults.Select(g => g.Topic);
                if (topics.Any())
                {
                    return topics.ToList();
                }
                return Enumerable.Empty<Topic>().ToList();
            }
            return Enumerable.Empty<Topic>().ToList();
        }

        // Returns a list of topics that are assigned as goals to the particular worker
        public List<Topic> GetAvailableTopics(Worker worker)
        {
            var topics = _edu.Topics.ToList();
            if (!topics.Any())
            {
                return Enumerable.Empty<Topic>().ToList();
            }
            // First check if worker has any assigned goals, if not, all topics are available
            var workerGoalsAsTopics = GetWorkerGoalsAsTopics(worker).ToList();
            if (!workerGoalsAsTopics.Any())
            {
                return topics;
            }

            topics.RemoveAll(t => workerGoalsAsTopics.Contains(t));
            return topics;
        }
    }
}
