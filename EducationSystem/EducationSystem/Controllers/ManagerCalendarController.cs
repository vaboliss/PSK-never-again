using EducationSystem.Data;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using EducationSystem.Static;
using EducationSystem.Views.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EducationSystem.Controllers
{
    [Authorize(Roles ="Manager")]
    public class ManagerCalendarController : Controller
    {
        private readonly EducationSystemDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IWorker _workerService;

        private ApplicationUser currentUser;

        public ManagerCalendarController(EducationSystemDbContext context, UserManager<ApplicationUser> userManager, IWorker workerService)
        {
            _context = context;
            _userManager = userManager;
            _workerService = workerService;
        }
        public async Task<IActionResult> Index()
        {
            await GetSuggestedTopics();
            ViewData["Subordinates"] = new SelectList(await GetManagerSubordinates(), nameof (Worker.Id), nameof(Worker.FirstName));
            return View();
        }

        // Returns manager subordinates
        public async Task<List<Worker>> GetManagerSubordinates()
        {
            currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return _workerService.GetCurrentWorkers(currentUser.WorkerId);
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
        public async Task<IActionResult> GetWorkerRestrictions()
        {
            Restriction restriction;
            currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var restrictions = _context.Restrictions.Where(r => r.WorkerId == currentUser.WorkerId);
            if (restrictions.Any())
            {
                restriction = restrictions.First();
            }
            else
            {
                restriction = new Restriction
                {
                    MaxConsecutiveDays = GlobalRestrictions.MaxConsecutiveDays,
                    MaxPerMonth = GlobalRestrictions.MaxPerMonth,
                    MaxPerQuarter = GlobalRestrictions.MaxPerQuarter,
                    MaxPerYear = GlobalRestrictions.MaxPerYear
                };
            }
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
            return View(Index());
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
            return View(Index());
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

        // Delete learning day entity
        [HttpDelete]
        public async Task<IActionResult> DeleteLearningDay([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                LearningDay learningDay = _context.Find<LearningDay>(eventModel.Id);
                if (learningDay == null)
                {
                    return NotFound();
                }
                _context.Remove(learningDay);
                _context.SaveChanges();
                var jsonData = JsonSerializer.Serialize(eventModel);
                return Json(jsonData);
            }
            return NotFound();
        }
    }
}
