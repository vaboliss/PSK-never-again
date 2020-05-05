using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Models.Entities;

namespace Infrastructure.provider
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
            var databaseResults = _edu.Worker_Topics.Where(x => x.TopicId == topicId);

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
