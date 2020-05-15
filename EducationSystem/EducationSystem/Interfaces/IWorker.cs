using System.Collections.Generic;
using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface IWorker
    {
        public List<Worker> GetAvailableWorkers(int managerId);
        public bool  AssignWorkers(int managerId, int workerId);
        public List<Worker> GetCurrentWorkers(int managerId);
    }

}
