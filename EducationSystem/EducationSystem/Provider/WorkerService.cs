using System.Collections.Generic;
using System.Linq;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;
using System;

namespace Infrastructure.Provider
{
    public class WorkerService : IWorker
    {
        private readonly EducationSystemDbContext _edu;

        public WorkerService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }

        public int CreateWorkerRId(Worker worker)
        {
            _edu.Workers.Add(worker);
            _edu.SaveChanges();
            return worker.Id;
        }
        public void RemoveWorkerById(int id)
        {
            var databaseResult = _edu.Workers.Where(w => w.Id == id);

            if (databaseResult.Any())
            {
                _edu.Workers.Remove(databaseResult.First());
                _edu.SaveChanges();
            }
            throw new Exception("Worker doesn't exist");
        }
    }
}
