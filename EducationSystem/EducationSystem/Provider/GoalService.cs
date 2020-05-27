using EducationSystem.Data;

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
