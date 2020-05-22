using EducationSystem.Data;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
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
        /*
        public LearningDay CreateLearningDay(int id, int topicId, DateTime date, string comment)
        {
            Topic topic = _edu.Find<Topic>(topicId);
            if (topic == null)
            {
                return ;
            }
            LearningDay learningDay = new LearningDay();
            learningDay.Topic = topic;
            learningDay.TopicId = topic.Id;
            learningDay.Date = date;
            _edu.Add(learningDay);
            _edu.SaveChanges();
            return true;
        }*/
    }
}
