﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationSystem.Data;
using EducationSystem.Models;
using EducationSystem.Provider;
using EducationSystem.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Net;

namespace EducationSystem.Controllers
{
    [Authorize(Roles = "Manager")]
    public class TeamsController : Controller
    {
        private readonly EducationSystemDbContext _context;
        private readonly IWorker _workerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TeamsController(EducationSystemDbContext context, IWorker workerService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _workerService = workerService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {

            var username = HttpContext.User.Identity.Name;
            var currentUser = await _userManager.FindByNameAsync(username);
            if (currentUser == null)
            {
                return NotFound();
            }
            var allSubordinates = _workerService.getAllSubordinates(currentUser.WorkerId);
            var educationSystemDbContext = new List<Team>();
            educationSystemDbContext.Add(_context.Teams.Include(t => t.Manager).FirstOrDefault(t => t.WorkerId == currentUser.WorkerId));

            foreach (Worker subordinate in allSubordinates)
                educationSystemDbContext.AddRange(_context.Teams.Include(t => t.Manager).Where(t => t.Manager.Id == subordinate.Id).ToList());
            return View(educationSystemDbContext);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["CurrentWorkers"] = _workerService.GetCurrentWorkers(team.Manager.Id);
            ViewData["AvailableWorkers"] = new SelectList(_workerService.GetAvailableWorkers(team.Manager.Id), nameof(Worker.Id), nameof(Worker.FirstName));
            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            var workers = _context.Workers.ToList();
            IEnumerable<SelectListItem> selectList = from s in workers
                                                     select new SelectListItem
                                                     {
                                                         Value = s.Id.ToString(),
                                                         Text = s.FirstName + " " + s.LastName
                                                     };

            ViewData["WorkerId"] = new SelectList(selectList, "Value", "Text");
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeamName,WorkerId")] Team team)
        {

            if (ModelState.IsValid)
            {

                var isManager = _context.Teams.Where(t => t.WorkerId == team.WorkerId).ToList();
                if (isManager.Any())
                {
                    ModelState.AddModelError("", "This person already has a team");
                    var workers = _context.Workers.ToList();
                    IEnumerable<SelectListItem> selectList = from s in workers
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.Id.ToString(),
                                                                 Text = s.FirstName + " " + s.LastName
                                                             };

                    ViewData["WorkerId"] = new SelectList(selectList, "Value", "Text");
                    return View(team);

                }
                var username = HttpContext.User.Identity.Name;
                var currentUser = await _userManager.FindByNameAsync(username);
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.WorkerId == team.WorkerId);
                if (user == null)
                {
                    return NotFound();
                }
                await _userManager.RemoveFromRoleAsync(user, "Worker");
                await _userManager.AddToRoleAsync(user, "Manager");
                await _signInManager.RefreshSignInAsync(currentUser);
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Id", team.WorkerId);
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.Include(i => i.Manager)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Id", team.WorkerId);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, byte[] rowVersion)
        {
            var teamToUpdate = await _context.Teams.Include(i => i.Manager).Include(t => t.Manager.Subordinates).FirstOrDefaultAsync(m => m.Id == id);

            if (teamToUpdate == null)
            {
                Team deletedTeam = new Team();
                await TryUpdateModelAsync(deletedTeam);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The Team was deleted by another user.");
                ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Id", deletedTeam.WorkerId);
                return View(deletedTeam);
            }

            if (teamToUpdate.Manager.Subordinates.Any())
            {
                ModelState.AddModelError("", "You can't change team manager which has workers");
                ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Id", teamToUpdate.WorkerId);
                return View(teamToUpdate);
            }

            _context.Entry(teamToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Team>(
        teamToUpdate,
        "",
        s => s.TeamName, s => s.WorkerId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Team)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The Team was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Team)databaseEntry.ToObject();

                        if (databaseValues.TeamName != clientValues.TeamName)
                        {
                            ModelState.AddModelError("TeamName", $"Current value: {databaseValues.TeamName}");
                        }
                        if (databaseValues.WorkerId != clientValues.WorkerId)
                        {
                            Worker databaseWorker = await _context.Workers.FirstOrDefaultAsync(i => i.Id == databaseValues.WorkerId);
                            ModelState.AddModelError("WorkerId", $"Current value: {databaseWorker?.LastName + databaseWorker?.FirstName}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        teamToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Id", teamToUpdate.WorkerId);

            return View(teamToUpdate);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Team team)
        {

            try
            {
                var teamToDelete = _context.Teams.Include(t => t.Manager).Include(t => t.Manager.Subordinates)
                 .FirstOrDefault(m => m.Id == team.Id);
                if (teamToDelete == null)
                {
                    return RedirectToAction("Index");
                }
                if (teamToDelete.Manager.Subordinates.Any())
                {
                    ModelState.AddModelError("", "You can't delete a team which has workers");
                    return View(teamToDelete);
                }
                _context.Entry(teamToDelete).State = EntityState.Detached;
                _context.Attach(team);
                _context.Entry(team).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                var username = HttpContext.User.Identity.Name;
                var currentUser = await _userManager.FindByNameAsync(username);
                //var user = await _userManager.FindByIdAsync(manager.Id.ToString());
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.WorkerId == teamToDelete.WorkerId);
                if (user == null)
                {
                    return NotFound();
                }

                await _userManager.RemoveFromRoleAsync(user, "Manager");
                await _userManager.AddToRoleAsync(user, "Worker");
                await _signInManager.RefreshSignInAsync(currentUser);

                return RedirectToAction("Index");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = team.Id });
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(team);
            }
        }

        [HttpPost, ActionName("AssignWorker")]
        [ValidateAntiForgeryToken]
        public ActionResult AssignWorker(int? id, IFormCollection formCollection)
        {
            var team = _context.Teams
                .Include(t => t.Manager)
                .FirstOrDefault(m => m.Id == id);
            int workerId;
            int.TryParse(formCollection["WorkerId"], out workerId);
            if (_workerService.AssignWorkers(team.Manager.Id, workerId))
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("DeleteWorker")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteWorker(int? id, int? managerId)
        {
            if (id == null || managerId == null)
            {
                return NotFound();
            }
            var manager = _context.Workers.Include(t => t.Subordinates)
                 .FirstOrDefault(m => m.Id == managerId);
            if (manager == null)
            {
                return NotFound();
            }
            manager.Subordinates.Remove(_context.Workers.Find(id));

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddRestriction(int id)
        {

            var worker = await _context.Workers.Include(i => i.Restriction)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRestrictionPost(int id, [Bind("MaxConsecutiveDays,MaxPerMonth,MaxPerQuarter,MaxPerYear")] Restriction restriction)
        {
            var workerToupdate = await _context.Workers.Include(i => i.Restriction).FirstOrDefaultAsync(m => m.Id == id);

            if (workerToupdate == null)
            {
                return NotFound();
            }
            if (workerToupdate.Restriction == null)
            {
                Restriction newRestriction = new Restriction();
                workerToupdate.Restriction = newRestriction;
            }
            if (restriction.MaxConsecutiveDays != null)
            {
                workerToupdate.SetMaxConsecutiveDays(restriction.MaxConsecutiveDays);
            }
            if (restriction.MaxPerMonth != null)
            {
                workerToupdate.SetMaxPerMonth(restriction.MaxPerMonth);
            }
            if (restriction.MaxPerQuarter != null)
            {
                workerToupdate.SetMaxPerQuarter(restriction.MaxPerQuarter);
            }
            if (restriction.MaxPerYear != null)
            {
                workerToupdate.SetMaxPerYear(restriction.MaxPerYear);
            }

            _context.Update(workerToupdate);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult GetWorkerTopics([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Worker worker = _context.Find<Worker>(id);
            if (worker == null)
            {
                return NotFound();
            }
            List<Topic> availableTopics = _workerService.GetAvailableTopics(worker);
            List <TopicCreateViewModel> data = new List<TopicCreateViewModel>();
            foreach (var topic in availableTopics)
            {
                var item = new TopicCreateViewModel();
                item.Id = topic.Id;
                item.Name = topic.Name;
                data.Add(item);
            }
            var jsonData = JsonSerializer.Serialize(data);
            return Json(jsonData);
        }

        [HttpPost]
        public IActionResult AssignGoalForWorker([FromBody] TopicCreateViewModel data)
        {
            if (data.Id == 0 || data.ParentId == 0)
            {
                return NotFound();
            }
            Worker worker = _context.Find<Worker>(data.Id);
            if (worker == null)
            {
                return NotFound();
            }
            var goalCreated = _workerService.AssignGoal(worker, data.ParentId);
            if (goalCreated)
            {
                return Json(data);
            }
            else
            {
                return View(HttpStatusCode.BadRequest);
            }
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
