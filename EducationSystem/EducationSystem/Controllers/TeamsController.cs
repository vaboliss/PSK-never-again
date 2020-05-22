using System;
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
using System.Data.Entity.Infrastructure;

namespace EducationSystem.Controllers
{
    public class TeamsController : Controller
    {
        private readonly EducationSystemDbContext _context;
        private readonly IWorker _workerService;

        public TeamsController(EducationSystemDbContext context, IWorker workerService)
        {
            _context = context;
            _workerService = workerService;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var educationSystemDbContext = _context.Teams.Include(t => t.Manager);
            return View(await educationSystemDbContext.ToListAsync());
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
                                                         Text = s.FirstName +" "+ s.LastName
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

                var isManager = _context.Teams.Where(t=> t.WorkerId==team.WorkerId).ToList();
                if (isManager.Any())
                {
                    ModelState.AddModelError("", "This person is already a manager");
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
        public async Task<IActionResult> Edit(int id,byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            
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

            if (teamToUpdate.Manager.Subordinates.Count() != 0)
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
                            ModelState.AddModelError("WorkerId", $"Current value: {databaseWorker?.LastName+ databaseWorker?.FirstName}");
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
        public async Task<IActionResult> Delete(int? id)
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
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.Include(t => t.Manager).Include(t => t.Manager.Subordinates)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team.Manager.Subordinates.Count() != 0)
            {
                ModelState.AddModelError("", "You can't delete a team which has workers");
                return View(team);
            }
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        public ActionResult DeleteWorker(int? id, int? managerId)
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


        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
