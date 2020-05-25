using EducationSystem.Data;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Views.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace EducationSystem.Controllers
{
    public class CalendarController : Controller
    {
        private readonly EducationSystemDbContext _context;

        private readonly ILearningDay _learningDayService;

        public CalendarController(EducationSystemDbContext context, ILearningDay learningDayService)
        {
            _context = context;
            _learningDayService = learningDayService;
        }
        public IActionResult Index()
        {
            GetSuggestedTopics();
            return View();
        }

        // Creates a list of learning days for the calendar to display
        [HttpGet]
        public IActionResult GetLearningDays()
        {
            List<EventViewModel> calendarEvents = new List<EventViewModel>();
            List<LearningDay> learningDays = _learningDayService.GetAllLearningDays(); // GetLearningDaysByWorker exists as well
            foreach (LearningDay day in learningDays)
            {
                EventViewModel tempEvent = new EventViewModel();
                tempEvent.Id = day.Id;
                tempEvent.Title = day.Topic.Name;
                tempEvent.Start = day.Date;
                calendarEvents.Add(tempEvent);
            }

            return Json(calendarEvents);
        }

        // Returns worker restrictions
        [HttpGet]
        public IActionResult GetWorkerRestrictions()        //  TO-DO: Worker Id should be equal to current logged in user Id
        {
            Worker worker = _context.Find<Worker>(1);
            if (worker == null)
            {
                return NotFound();
            }
            Restriction restriction = _context.Find<Restriction>(1);
            var jsonData = JsonSerializer.Serialize(restriction);
            return Json(jsonData);
        }

        // Creates a ViewBag of Suggested Topics aka Goals
        public void GetSuggestedTopics()
        {
            List<EventViewModel> suggestedTopics = new List<EventViewModel>();
            List<Goal> goals = _context.Goals.Include(ld => ld.Topic).ToList(); 
            foreach (Goal goal in goals)
            {
                EventViewModel tempEvent = new EventViewModel();
                tempEvent.Id = goal.Topic.Id;
                tempEvent.Title = goal.Topic.Name;
                suggestedTopics.Add(tempEvent);
            }
            ViewData["SuggestedTopics"] = suggestedTopics;
        }

        // Creates LearningDay entity from the calendar
        [HttpPost]
        public IActionResult CreateLearningDay([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                Topic topic = _context.Find<Topic>(eventModel.Id);
                Worker worker = _context.Find<Worker>(1);   // TO-DO: Should use logged in worker Id
                LearningDay learningDay = new LearningDay();
                learningDay.Topic = topic;
                learningDay.TopicId = topic.Id;
                learningDay.Worker = worker;
                learningDay.WorkerId = worker.Id;
                learningDay.Date = eventModel.Start;
                _context.Add(learningDay);
                _context.SaveChanges();
                return View(learningDay);
            }
            return View(Index());
        }

        // Creates LearningDay entity from the calendar
        [HttpPost]
        public IActionResult CreateLearningDayAndTopic([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                Worker worker = _context.Find<Worker>(1);   // TO-DO: Should use logged in worker Id

                Topic topic = new Topic();
                topic.Name = eventModel.TopicName;
                topic.Description = eventModel.TopicDescription;
                _context.Add(topic);

                LearningDay learningDay = new LearningDay();
                learningDay.Topic = topic;
                learningDay.TopicId = topic.Id;
                learningDay.Worker = worker;
                learningDay.WorkerId = worker.Id;
                learningDay.Date = eventModel.Start;
                _context.Add(learningDay);

                _context.SaveChanges();
                return View(learningDay);
            }
            return View(Index());
        }

        // Returns specified day info
        [HttpPost]
        public IActionResult GetDayInfo([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                Worker worker = _context.Find<Worker>(1);
                if (worker == null)
                {
                    return NotFound();
                }
                var learningDays = _context.LearningDays.Where(ld => ld.Date == eventModel.Start && ld.Topic.Name == eventModel.TopicName).Include(ld => ld.Topic).ToList();
                if (!learningDays.Any())
                {
                    return NotFound();
                }
                var learningDay = learningDays.First();

                EventViewModel dayInfo = new EventViewModel();
                dayInfo.Title = worker.FirstName + " " + worker.LastName;
                dayInfo.TopicName = learningDay.Topic.Name;
                dayInfo.Comments = learningDay.Comment;
                dayInfo.Id = learningDay.Id;

                var jsonData = JsonSerializer.Serialize(dayInfo);
                return Json(jsonData);
            }
            return NotFound();
        }

        // Updates learning day entity with changes made to comments
        [HttpPut]
        public IActionResult UpdateComments([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                Worker worker = _context.Find<Worker>(1);
                if (worker == null)
                {
                    return NotFound();
                }
                LearningDay learningDay = _context.Find<LearningDay>(eventModel.Id);
                if (learningDay == null)
                {
                    return NotFound();
                }
                learningDay.Comment = eventModel.Comments;
                _context.SaveChanges();
                var jsonData = JsonSerializer.Serialize(eventModel);
                return Json(jsonData);
            }
            return NotFound();
        }
    }
}