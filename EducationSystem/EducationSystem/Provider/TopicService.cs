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
            var topic = _edu.Topics.Where(t=> t.Id == id);

            if (topic.Any())
            {
                return topic.First();
            }
            return null;
        }

        public List<Worker> GetWorkersByTopic(int topicId)
        {
            var workersByTopic = _edu.WorkerTopics.Where(x => x.TopicId == topicId);

            if (workersByTopic.Any())
            {
                var workers = workersByTopic.Select(w => w.Worker);
                if (workers.Any())
                {
                    return workers.ToList();
                }
                return Enumerable.Empty<Worker>().ToList();
            }
            return Enumerable.Empty<Worker>().ToList();
        }
    }
}
