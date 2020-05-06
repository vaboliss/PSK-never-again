using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Models.Entities;

namespace Infrastructure.provider
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
