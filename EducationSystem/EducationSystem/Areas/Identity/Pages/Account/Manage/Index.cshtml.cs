using System;
using System.Linq;
using System.Threading.Tasks;
using EducationSystem.Data;
using EducationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EducationSystemDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, EducationSystemDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        { 
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            
            Username = userName;
            Email = user.Email;
            Input = new InputModel()
            {
                FirstName = this.FirstName,
                LastName = this.LastName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var workers = await _context.Workers.Where(x => x.Id == user.WorkerId).ToListAsync();
            if (workers != null)
            { 
                FirstName = workers[0].FirstName;
                LastName = workers[0].LastName;
            }
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var worker = _context.Workers.Where(x => x.Id == user.WorkerId).First();
            if (user == null || worker == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (worker.FirstName != Input.FirstName || worker.LastName != Input.LastName)
            {
                try
                {
                    worker.FirstName = Input.FirstName;
                    worker.LastName = Input.LastName;
                    _context.Update(worker);
                    _context.SaveChanges();

                    StatusMessage = "Your profile has been updated";
                    return RedirectToPage();
                }
                catch(Exception ex)
                { 
                    ModelState.AddModelError(string.Empty, "Failed to update the profile");
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
