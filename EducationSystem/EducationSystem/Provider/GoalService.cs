using EducationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducationSystem.Provider
{
    public class GoalService
    {
        private readonly EducationSystemDbContext _edu;

        public GoalService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }
    }
}
