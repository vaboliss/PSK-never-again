using System.Collections.Generic;
using System.Linq;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Provider
{
    public class WorkerService : IWorker
    {
        private readonly EducationSystemDbContext _edu;

        public WorkerService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }


        public List<Worker> GetAvailableWorkers(int managerId)
        {
            var allworkers = _edu.Workers.Include(t => t.Subordinates).ToList();

            var availableWorkers = _edu.Workers.ToList();
            var manager = _edu.Workers.Include(t => t.Subordinates)
                .FirstOrDefault(m => m.Id == managerId);

            foreach (Worker worker in allworkers)
            {
                availableWorkers.RemoveAll(w => worker.Subordinates.Contains(w));
            }

            availableWorkers.RemoveAll(w => w.Subordinates.Count != 0);
            availableWorkers.Remove(manager);
            // TO DO: Remove All higher lever workers.
            //availableWorkers.RemoveAll(w => manager.Subordinates.Contains(w));



            if (availableWorkers.Any())
            {
                return availableWorkers;
            }
            return Enumerable.Empty<Worker>().ToList();
        }
        public List<Worker> GetCurrentWorkers(int managerId)
        {


            var manager = _edu.Workers.Include(t => t.Subordinates)
                 .FirstOrDefault(m => m.Id == managerId);

            var subordinates = manager.Subordinates;

            if (subordinates.Any())
            {
                return subordinates.ToList();
            }
            return Enumerable.Empty<Worker>().ToList();
        }

        public bool AssignWorkers(int managerId, int workerId)
        {


            //var manager = _edu.Workers.Find(managerId);
            var worker = _edu.Workers.Find(workerId);

            var manager = _edu.Workers.Include(t => t.Subordinates)
                .FirstOrDefault(m => m.Id == managerId);

            if (manager == null || worker==null)
            {
                return false;
            }

            manager.Subordinates.Add(worker);
            _edu.Update(manager);
            _edu.SaveChanges();
            return true;
            
        }


    }
}
