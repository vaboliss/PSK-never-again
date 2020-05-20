using EducationSystem.Data;
using EducationSystem.Models;
using EducationSystem.Views.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;

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
            return View();
        }

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
    }
}