using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EducationSystem.Data;
using EducationSystem.Models;
using EducationSystem.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using X.PagedList;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EducationSystem.Controllers
{
    [Authorize]
    public class TopicsController : Controller
    {

        private readonly EducationSystemDbContext _context;
        private readonly ITopic _topicService;
        private readonly IWorker _workerService;
        private readonly UserManager<ApplicationUser> _userManager;
        public TopicsController(EducationSystemDbContext context, ITopic topicService, IWorker workerService,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _topicService = topicService;
            _userManager = userManager;
            _workerService = workerService;
        }

        // GET: Topics
        public async Task<IActionResult> Index(string sortOrder, int? emptySearch, string searchString, string currentFilter, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null || emptySearch == default(int))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewBag.CurrentFilter = searchString;
            var topics = await _topicService.GetAllTopics();
            var username = HttpContext.User.Identity.Name;
            var user =  await _userManager.FindByNameAsync(username);
            var worker = await _context.Workers.FirstOrDefaultAsync(m => m.Id == user.WorkerId);
            var workerTopic = _workerService.GetWorkersTopics(worker);
            var goalTopic = _context.Goals.Where(g => g.Worker == worker).Select(a => a.Topic).ToList() ;
            var modelTopics = MapTopicList(topics,workerTopic,goalTopic);


            if (!String.IsNullOrEmpty(searchString))
            {
                modelTopics = modelTopics.Where(s => s.Name.Contains(searchString)).ToList();
            }

            switch (sortOrder) {
                case "name_desc":
                    modelTopics = modelTopics.OrderByDescending(s => s.Name).ToList();
                    break;
                default:
                    modelTopics = modelTopics.OrderBy(s => s.Name).ToList();
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(modelTopics.ToPagedList(pageNumber,pageSize));
        }

        public List<TopicModel> MapTopicList(List<Topic> topics,List<Topic> workerTopics,List<Topic> goals)
        {
            List<TopicModel> topicModelList = new List<TopicModel>();
            foreach (var topic in topics)
            {
                TopicModel tempModel = new TopicModel();
                if (workerTopics.Contains(topic))
                {
                    tempModel.Learned = true;

                }
                else {
                    tempModel.Learned = false;
                }
                if (goals.Contains(topic))
                {
                    Console.WriteLine("hello");
                    tempModel.GoalsLearned=true;
                }
                else {
                    tempModel.GoalsLearned = false;
                }
                tempModel.Description = topic.Description;
                tempModel.Id = topic.Id;
                tempModel.Name = topic.Name;
                topicModelList.Add(tempModel);

            }


            return topicModelList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Learn(int topicId,bool learned,string type,string place) {


            var topic = _topicService.GetTopicById(topicId);
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var worker = await _context.Workers.FirstOrDefaultAsync(m => m.Id == user.WorkerId);
            if (type=="learnUnlearn")
            {
                if (!learned)
                {
                    _workerService.AssingLearned(worker, topic);
                }
                else
                {
                    _workerService.RemoveLearned(worker, topic);
                }
            }
            else
            {
                if (!learned)
                {
                    _workerService.AssignGoal(worker, topicId);
                }
                else {
                    Goal goal = new Goal();
                    goal = _context.Goals.Where(g => g.Topic == topic && g.Worker == worker).FirstOrDefault();
                    _context.Goals.Remove(goal);
                    _context.SaveChanges();
                }

            }
            if (place.Equals("index"))
            {
                return Redirect(nameof(Index));
            }
            else {
                return RedirectToAction(nameof(Details), new { id = topic.Parent.Id });
            }



        }


        // GET: Topics/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            Console.WriteLine(id);
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.Include(m => m.SubTopics).Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (topic == null)
            {
                return NotFound();
            }
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var worker = await _context.Workers.FirstOrDefaultAsync(m => m.Id == user.WorkerId);
            var workerTopic = _workerService.GetWorkersTopics(worker);
            var goalTopic = _context.Goals.Where(g => g.Worker == worker).Select(a => a.Topic).ToList();
            var modelTopics = MapTopicList(topic.SubTopics.ToList(), workerTopic, goalTopic);

            ViewBag.subtopics = modelTopics;


            return View(topic);
        }


        // GET: Topics/Create
        public IActionResult Create(int? id)
        {
            TopicCreateViewModel tcm = new TopicCreateViewModel();
            if (id == null)
            {
                List<Topic> topiclist = _context.Topics.ToListAsync().Result;
                topiclist.Insert(0, new Topic() { Id = -1, Name = "none" });

                Topic parent= null;
                ViewBag.Parent = parent;
            }
            else {
                tcm.ParentId = (int)id;
                Topic parent = _context.Topics.FirstOrDefaultAsync(m => m.Id == (int)id).Result;
                ViewBag.Parent = parent;

            }
            
            return View(tcm);
        }

        // POST: Topics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicCreateViewModel topic)
        
        {
            Topic topicToCreate = new Topic() { Name = topic.Name, Description = topic.Description };
            int redirect = default(int);
            if (topic.ParentId != default(int)) {
                redirect = topic.Id;
                }

            if (topic.ParentId != default(int))
            {
                topicToCreate.Parent = _topicService.GetTopicById(topic.ParentId);
            }

            if (ModelState.IsValid)
            {

                _context.Add(topicToCreate);
                await _context.SaveChangesAsync();
                if (redirect == default(int))
                {
                    return RedirectToAction(nameof(Index));
                }
                else {
                    return RedirectToAction(nameof(Details),new { id = redirect });
                }
            }

            List<Topic> topiclist = _context.Topics.ToListAsync().Result;
            topiclist.Insert(0, new Topic() { Id = -1, Name = "none" });

            SelectList sl2 = new SelectList(topiclist, "Id", "Name"); ;
            ViewBag.TopicList = sl2;

            return View(topic);
        }

        // GET: Topics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            return View(topic);
        }

        // POST: Topics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Topic topic)
        {
            if (id != topic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topic.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                    return RedirectToAction(nameof(Details), new { id = id });
                
            }

            return View(topic);
        }

        // GET: Topics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        public async Task<IActionResult> Teams(int? Id,int? teamId)
        {
            var topic = _topicService.GetTopicById((int)Id);
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var worker = await _context.Workers.FirstOrDefaultAsync(m => m.Id == user.WorkerId);
            var a = putIntoTeams(worker, (int)Id);
            ViewBag.teamModel = a;
            return View(topic);
        }

        public List<TeamModel> putIntoTeams(Worker worker, int topicId)
        {
            List<TeamModel> topicModel = new List<TeamModel>();
            List<Worker> workers = _workerService.GetAllSubordinates(worker.Id, topicId);
            List<List<Worker>> sameTeam = new List<List<Worker>>();


            foreach (var w in workers)
            {
                if (topicModel.Any(a => a.manager == w.Parent))
                {
                    topicModel.Where(a => a.manager == w.Parent).FirstOrDefault().workers.Add(w);
                }
                else
                {
                    var tempTeamModel = new TeamModel();
                    tempTeamModel.manager = w.Parent;
                    tempTeamModel.workers.Add(w);
                    topicModel.Add(tempTeamModel);
                }
            }
            foreach (var t in topicModel)
            {
                t.team = _context.Teams.Where(teams => teams.WorkerId == t.manager.Id).FirstOrDefault();
                t.teamSize = _workerService.GetSubordinatesCount(t.manager.Id);
            }
                return topicModel;
        }


        public ActionResult Tree(int Id)
        {

            if (Id == null) {
                return NotFound();
            }
            Topic topic = _topicService.GetTopicById((int)Id);

            return View(topic);
        }


        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }
        [HttpGet]
        public JsonResult AjaxMethod(int id)
        {
            Topic topic = _topicService.GetTopicById(id);

            topic.SubTopics = getAllSubordinates(topic.Id);
            Console.WriteLine(topic.SubTopics.Count);
            List<Object> str = new List<object>();
            str.Add(new object[] { "Topic tree" });

            str.AddRange(getTopics(topic,topic.Name));
            return Json(str);
        }
        public List<Topic> getAllSubordinates(int topicId)
        {
            var topic = _context.Topics.Include(t => t.SubTopics)
                .FirstOrDefault(m => m.Id == topicId);
            topic.Name = topic.Name.Replace(" ", "_");
            List<Topic> result = new List<Topic>();
            foreach (var subtopics in topic.SubTopics)
            {
                result.Add(subtopics);
                if (subtopics.SubTopics != null)
                {
                    result[result.Count-1].SubTopics=getAllSubordinates(subtopics.Id);
                }
                subtopics.Name = subtopics.Name.Replace(" ", " ");
            }

            return result;
        }

        public List<Object> getTopics(Topic topics,string name)
        {

            List<Object> objects = new List<Object>();
            objects.Add(new Object[] { name });
            foreach (var v in topics.SubTopics){
                string temp = new String(name);
                temp +=" "+v.Name;
                if (v.SubTopics != null)
                {
                   objects.AddRange(getTopics(v, temp));
                }
            }


        return objects;
        }
    }
}
