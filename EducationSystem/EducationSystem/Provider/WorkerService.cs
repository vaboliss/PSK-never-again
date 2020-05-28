using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using EducationSystem.Static;

namespace EducationSystem.Provider
{
    public class WorkerService : IWorker
    {
        private readonly EducationSystemDbContext _edu;
        private readonly ITopic _topicService;

        public WorkerService(EducationSystemDbContext edu, ITopic topicService)
        {
            _edu = edu;
            _topicService = topicService;
        }

        // Assigns goal to worker, topicId needed
        public bool AssignGoal (Worker worker, int topicId)
        {
            Topic topic = _topicService.GetTopicById(topicId);
            if (topic is null)
                return false;
            Goal goalToCreate = new Goal();
            goalToCreate.Worker = worker;
            goalToCreate.Topic = topic;
            try
            {
                _edu.Add(goalToCreate);
            }
            catch
            {
                return false;
            }
            _edu.SaveChanges();
            return true;
        }
        public bool AssingLearned(Worker worker,Topic topic)
        {
            if (topic is null||worker is null)
                return false;

            WorkerTopic workerTopic = new WorkerTopic();
            workerTopic.Worker = worker;
            workerTopic.Topic = topic;
            if (_edu.WorkerTopics.Any(o => o.Topic == workerTopic.Topic && o.Worker == workerTopic.Worker)){
                return false;
            }
            try
            {
                _edu.WorkerTopics.Add(workerTopic);
            }
            catch
            {
                return false;
            }
            _edu.SaveChanges();
            return true;
        }
        public bool RemoveLearned(Worker worker, Topic topic)
        {
            if (topic is null || worker is null)
                return false;

            WorkerTopic workerTopic = new WorkerTopic();
            workerTopic.Worker = worker;
            workerTopic.Topic = topic;
            if (_edu.WorkerTopics.Any(o => o.Topic == workerTopic.Topic && o.Worker == workerTopic.Worker))
            {
                try
                {
                    _edu.WorkerTopics.Remove(workerTopic);
                }
                catch
                {
                    return false;
                }

                _edu.SaveChanges();
            }
            return true;
        }
        public List<Topic> GetWorkersTopics(Worker worker)
        {
            var topicList = _edu.WorkerTopics.Where(g => g.Worker == worker).Select(g => g.Topic).ToList();
            return topicList;
        }
        // Returns a list of topics that are assigned as goals to the particular worker
        public List<Topic> GetWorkerGoalsAsTopics (Worker worker)
        {
            var workerGoals = _edu.Goals.Where(g => g.WorkerId == worker.Id);
            if (workerGoals.Any())
            {
                var topics = workerGoals.Select(g => g.Topic);
                if (topics.Any())
                {
                    return topics.ToList();
                }
                return Enumerable.Empty<Topic>().ToList();
            }
            return Enumerable.Empty<Topic>().ToList();
        }

        // Returns a list of topics that are assigned as goals to the particular worker
        public List<Topic> GetAvailableTopics(Worker worker)
        {
            var topics = _edu.Topics.ToList();
            if (!topics.Any())
            {
                return Enumerable.Empty<Topic>().ToList();
            }
            // First check if worker has any assigned goals, if not, all topics are available
            var workerGoalsAsTopics = GetWorkerGoalsAsTopics(worker);
            if (!workerGoalsAsTopics.Any())
            {
                return topics;
            }

            topics.RemoveAll(t => workerGoalsAsTopics.Contains(t));
            return topics;
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
        public int GetSubordinatesCount(int workerId)
        {
            var worker = _edu.Workers.Include(s => s.Subordinates).Where(w => w.Id == workerId).FirstOrDefault();

            return worker.Subordinates.Count;
        
        }
        public List<Worker> GetAllSubordinates(int id, int topicId)
        {
            List<Worker> workerList = new List<Worker>();
            var worker= _edu.Workers.Include(w=>w.TeamManager).Include(s=>s.Subordinates).Where(w => w.Id == id).FirstOrDefault();

            if (worker.Subordinates != null)
            {

                foreach (var w in worker.Subordinates)
                {
                    if (_edu.Goals.Where(wrk => wrk.TopicId == topicId && wrk.WorkerId==w.Id).FirstOrDefault() != null)
                    {

                        workerList.Add(w);
                    }
                    if (_edu.Workers.Any(a => a.Subordinates!=null));
                    {
                        workerList.AddRange(GetAllSubordinates(w.Id, topicId));
                    }
                }
            }

            return workerList;
        }
        public bool AssignWorkers(int managerId, int workerId)
        {
            //var manager = _edu.Workers.Find(managerId);
            var worker = _edu.Workers.Find(workerId);

            var manager = _edu.Workers.Include(t => t.Subordinates)
                .FirstOrDefault(m => m.Id == managerId);

            if (manager == null || worker == null)
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
