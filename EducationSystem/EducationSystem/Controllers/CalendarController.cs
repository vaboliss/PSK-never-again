using EducationSystem.Data;
using EducationSystem.Models;
using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        [HttpGet]
        public IActionResult GetLearningDays()
        {
            List<LearningDay> learningDays = _context.LearningDays.ToList(); // TO-DO: Should GET the learning days for the specific user and/or his subordinates

            return Json(learningDays);
        }
    }
}