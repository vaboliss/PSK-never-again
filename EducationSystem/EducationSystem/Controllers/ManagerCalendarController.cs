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

        private readonly ILearningDay _learningDayService;

        public ManagerCalendarController(EducationSystemDbContext context, UserManager<ApplicationUser> userManager, IWorker workerService, ILearningDay learningDayService)
        {
            _context = context;
            _userManager = userManager;
            _workerService = workerService;
            _learningDayService = learningDayService;
        }
        public async Task<IActionResult> Index()
        {
            List<Worker> workers = await GetManagerSubordinates();
            IEnumerable<SelectListItem> selectList = from s in workers
                                                     select new SelectListItem
                                                     {
                                                         Value = s.Id.ToString(),
                                                         Text = s.FirstName + " " + s.LastName
                                                     };
            ViewData["Subordinates"] = new SelectList(selectList, "Value", "Text");
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
        public IActionResult GetLearningDays([FromQuery] int subordinateId)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            List<EventViewModel> calendarEvents = new List<EventViewModel>();
            var learningDays = _context.LearningDays.Where(ld => ld.WorkerId == subordinateId).Include(ld => ld.Topic).ToList(); // GetLearningDaysByWorker exists as well
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
        public IActionResult GetWorkerRestrictions([FromQuery] int subordinateId)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            Restriction restriction;
            var restrictions = _context.Restrictions.Where(r => r.WorkerId == subordinateId);
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
        [HttpGet]
        public IActionResult GetSuggestedTopics([FromQuery] int subordinateId)
        {
            List<EventViewModel> suggestedTopics = new List<EventViewModel>();
            List<Goal> goals = _context.Goals.Where(ld => ld.WorkerId == subordinateId).Include(ld => ld.Topic).ToList();
            foreach (Goal goal in goals)
            {
                EventViewModel tempEvent = new EventViewModel();
                tempEvent.Id = goal.Topic.Id;
                tempEvent.Title = goal.Topic.Name;
                suggestedTopics.Add(tempEvent);
            }
            return Json(suggestedTopics);
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
                learningDay.Worker = _context.Find<Worker>(eventModel.WorkerId);
                learningDay.WorkerId = eventModel.WorkerId;
                learningDay.Date = eventModel.Start;
                _context.Add(learningDay);
                _context.SaveChanges();
                _learningDayService.SendMail(learningDay);
                var jsonData = JsonSerializer.Serialize(eventModel);
                return Json(jsonData);
            }
            return NotFound();
        }

        // Creates LearningDay and new Topic from the calendar
        [HttpPost]
        public IActionResult CreateLearningDayAndTopic([FromBody] EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                Worker worker = _context.Find<Worker>(eventModel.WorkerId);
                if (worker == null)
                {
                    return NotFound();
                }
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
                _learningDayService.SendMail(learningDay);
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
                var learningDays = _context.LearningDays.Where(ld => ld.Date == eventModel.Start && ld.Topic.Name == eventModel.TopicName && ld.WorkerId == eventModel.WorkerId).Include(ld => ld.Topic).ToList();
                if (!learningDays.Any())
                {
                    return NotFound();
                }
                var learningDay = learningDays.First();
                Worker worker = _context.Find<Worker>(eventModel.WorkerId);
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
