using EducationSystem.Data;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Views.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace EducationSystem.Controllers
{
    [Authorize(Roles = "Worker")]
    public class CalendarController : Controller
    {
        private readonly EducationSystemDbContext _context;

        private readonly ILearningDay _learningDayService;

        private readonly UserManager<ApplicationUser> _userManager;

        private ApplicationUser currentUser;

        public CalendarController(EducationSystemDbContext context, ILearningDay learningDayService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _learningDayService = learningDayService;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            await GetSuggestedTopics();
            return View();
        }

        // Creates a list of learning days for the calendar to display
        [HttpGet]
        public async Task<IActionResult> GetLearningDays()
        {
            currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            List<EventViewModel> calendarEvents = new List<EventViewModel>();
            var learningDays = _context.LearningDays.Where(ld => ld.WorkerId == currentUser.WorkerId).Include(ld => ld.Topic).ToList(); // GetLearningDaysByWorker exists as well
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
        public async Task<IActionResult> GetWorkerRestrictions()        //  TO-DO: Worker Id should be equal to current logged in user Id
        {
            currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var restrictions = _context.Restrictions.Where(r => r.WorkerId == currentUser.WorkerId);
            if (!restrictions.Any())
            {
                return NotFound();
            }
            Restriction restriction = restrictions.First();
            var jsonData = JsonSerializer.Serialize(restriction);
            return Json(jsonData);
        }

        // Creates a ViewBag of Suggested Topics aka Goals
        public async Task GetSuggestedTopics()
        {
            currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            List<EventViewModel> suggestedTopics = new List<EventViewModel>();
            List<Goal> goals = _context.Goals.Where(ld => ld.WorkerId == currentUser.WorkerId).Include(ld => ld.Topic).ToList(); 
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
        public async Task<IActionResult> CreateLearningDay([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                Topic topic = _context.Find<Topic>(eventModel.Id);
                LearningDay learningDay = new LearningDay();
                learningDay.Topic = topic;
                learningDay.TopicId = topic.Id;
                learningDay.Worker = _context.Find<Worker>(currentUser.WorkerId);
                learningDay.WorkerId = currentUser.WorkerId;
                learningDay.Date = eventModel.Start;
                _context.Add(learningDay);
                _context.SaveChanges();
                var jsonData = JsonSerializer.Serialize(eventModel);
                return Json(jsonData);
            }
            return View(IndexAsync());
        }

        // Creates LearningDay and new Topic from the calendar
        [HttpPost]
        public async Task<IActionResult> CreateLearningDayAndTopic([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                Topic topic = new Topic();
                topic.Name = eventModel.TopicName;
                topic.Description = eventModel.TopicDescription;
                _context.Add(topic);

                LearningDay learningDay = new LearningDay();
                learningDay.Topic = topic;
                learningDay.TopicId = topic.Id;
                learningDay.Worker = _context.Find<Worker>(currentUser.WorkerId);
                learningDay.WorkerId = currentUser.WorkerId;
                learningDay.Date = eventModel.Start;
                _context.Add(learningDay);

                _context.SaveChanges();
                return View(learningDay);
            }
            return View(IndexAsync());
        }

        // Returns specified day info
        [HttpPost]
        public async Task<IActionResult> GetDayInfo([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var learningDays = _context.LearningDays.Where(ld => ld.Date == eventModel.Start && ld.Topic.Name == eventModel.TopicName).Include(ld => ld.Topic).ToList();
                if (!learningDays.Any())
                {
                    return NotFound();
                }
                var learningDay = learningDays.First();
                Worker worker = _context.Find<Worker>(currentUser.WorkerId);
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
        public async Task<IActionResult> UpdateComments([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
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