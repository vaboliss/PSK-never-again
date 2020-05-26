using EducationSystem.Data;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EducationSystem.Provider
{
    public class LearningDayService : ILearningDay
    {
        private readonly EducationSystemDbContext _edu;

        public LearningDayService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }

        // Returns all learning days
        public List<LearningDay> GetAllLearningDays()
        {
            List<LearningDay> learningDays = _edu.LearningDays.Include(ld => ld.Topic).ToList();
            return learningDays;
        }

        // Returns all learning days by specified worker
        public List<LearningDay> GetLearningDaysByWorker(Worker worker)
        {
            List<LearningDay> learningDays = _edu.LearningDays.Where(ld => ld.WorkerId == worker.Id).Include(ld => ld.Topic).ToList();
            return learningDays;
        }
    }
}
