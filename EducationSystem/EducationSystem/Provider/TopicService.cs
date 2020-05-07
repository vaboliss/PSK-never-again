using System.Linq;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;

namespace EducationSystem.Provider
{
    public class TopicService : ITopic
    {
        private readonly EducationSystemDbContext _edu;

        public TopicService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }

        public Topic GetTopicById(int id)
        {
            var databaseResult = _edu.Topics.Where(t=> t.Id == id);

            if (databaseResult.Any())
            {
                return databaseResult.First();
            }
            return null;
        }
    }
}
