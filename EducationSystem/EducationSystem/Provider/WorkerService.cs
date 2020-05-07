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
    }
}
