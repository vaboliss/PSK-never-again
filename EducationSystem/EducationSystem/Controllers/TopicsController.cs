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

namespace EducationSystem.Controllers
{
    public class TopicsController : Controller
    {
        private readonly EducationSystemDbContext _context;

        private readonly ITopic _topicService;
        public TopicsController(EducationSystemDbContext context, ITopic topicService)
        {
            _context = context;
            _topicService = topicService;
        }

        // GET: Topics
        public async Task<ActionResult> Index(string sortOrder,int? emptySearch,string searchString, string currentFilter, int? page)
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

            if (!String.IsNullOrEmpty(searchString))
            {
                topics = topics.Where(s => s.Name.Contains(searchString)).ToList();
            }

            switch (sortOrder) {
                case "name_desc":
                    topics = topics.OrderByDescending(s => s.Name).ToList();
                    break;
                default:
                    topics = topics.OrderBy(s => s.Name).ToList();
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(topics.ToPagedList(pageNumber,pageSize));
        }

        // GET: Topics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.Include(m=>m.SubTopics)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // GET: Topics/Create
        public IActionResult Create(int? id)
        {
            TopicCreateViewModel tcm=new TopicCreateViewModel();
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
        public async Task<IActionResult> Create( TopicCreateViewModel topic)
        
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
        public async Task<IActionResult> Edit( int id, [Bind("Id,Name,Description")] Topic topic)
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

        // POST: Topics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

   

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }
    }
}
