using System.Collections.Generic;
using System.Linq;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;

namespace Infrastructure.Provider
{
    public class WorkerService : IWorker
    {
        private readonly EducationSystemDbContext _edu;

        public WorkerService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }
        public List<Worker> GetWorkersByTopic(int topicId)
        {
            var databaseResults = _edu.WorkerTopics.Where(x => x.TopicId == topicId);

            if (databaseResults.Any())
            {
                var workers = databaseResults.Select(w=> w.Worker);
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
