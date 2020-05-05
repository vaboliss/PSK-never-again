using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public List<Worker> GetWorkersByTopic(Topic topic)
        {
            var databaseResults = _edu.Workers.Where(w => w.LearntTopics.Contains(topic));

            if (databaseResults.Any())
            {
                return databaseResults.ToList();
            }

            return null;
        }
    }
}
