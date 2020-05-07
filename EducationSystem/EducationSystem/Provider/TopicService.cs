using System.Linq;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;
using System.Collections.Generic;

namespace EducationSystem.Provider
{
    public class TopicService : ITopic
    {
        private readonly EducationSystemDbContext _edu;

        public TopicService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }

        public Topic GetTopicById(int id)
        {
            var databaseResult = _edu.Topics.Where(t=> t.Id == id);

            if (databaseResult.Any())
            {
                return databaseResult.First();
            }
            return null;
        }

        public List<Worker> GetWorkersByTopic(int topicId)
        {
            var databaseResults = _edu.WorkerTopics.Where(x => x.TopicId == topicId);

            if (databaseResults.Any())
            {
                var workers = databaseResults.Select(w => w.Worker);
                if (workers.Any())
                {
                    return workers.ToList();
                }
                return null;
            }
            return null;
        }
    }
}
