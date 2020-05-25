using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EducationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EducationSystem.Areas.Identity.Pages.Account.Manage
{
    public partial class ChangeRoleModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ChangeRoleModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public string Role { get; set; }

        [BindProperty]
        public RoleInputModel Input { get; set; }

        public class RoleInputModel
        {
            [Required]
            public string CurrentRole;
        }

        private async Task LoadAsync(ApplicationUser user)
        { 
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var listOfRoles = await _userManager.GetRolesAsync(user);
            Input = new RoleInputModel()
            {
                CurrentRole = listOfRoles.First()
            };
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            var listOfRoles = await _userManager.GetRolesAsync(user);
            if (Role == "" || Role == null)
            {
                StatusMessage = "Select role that you want to change";
                return RedirectToPage();
            }
            if (listOfRoles.First() != Role)
            {
                var removeRoleStatus = await _userManager.RemoveFromRoleAsync(user, listOfRoles.First());
                if (removeRoleStatus.Succeeded)
                {
                    var isRoleExists = await _roleManager.RoleExistsAsync("Manager");
                    if (!isRoleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Manager"));
                    }
                    var addRoleStatus = await _userManager.AddToRoleAsync(user, Role);
                    if (addRoleStatus.Succeeded)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        StatusMessage = "Role was changed";
                        return RedirectToPage();
                    }
                }
                StatusMessage = "Role wasn't changed";
                return RedirectToPage();

            }
            else
            {
                StatusMessage =  "Role are identical";
                return RedirectToPage();
            }

            return Page();
        }
    }
}
