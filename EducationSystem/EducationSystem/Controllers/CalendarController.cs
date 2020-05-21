using EducationSystem.Data;
using EducationSystem.Models;
using EducationSystem.Views.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EducationSystem.Controllers
{
    public class CalendarController : Controller
    {
        private readonly EducationSystemDbContext _context;

        public CalendarController(EducationSystemDbContext context)
        {
            _context = context;
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
            List<LearningDay> learningDays = _context.LearningDays.Include(ld => ld.Topic).ToList(); // TO-DO: Should GET the learning days for the specific user and/or his subordinates
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
                LearningDay learningDay = new LearningDay();
                learningDay.Topic = topic;
                learningDay.TopicId = topic.Id;
                learningDay.Date = eventModel.Start;
                _context.Add(learningDay);
                _context.SaveChanges();
                return View(learningDay);
            }
            return View(Index());
        }
    }
}