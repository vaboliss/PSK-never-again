using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using EducationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace EducationSystem.Areas.Identity.Pages.Account.Manage
{
    public partial class UsernameModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsernameModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public UsernameInputModel Input { get; set; }

        public class UsernameInputModel
        {
            [Required]
            [Display(Name = "New Username")]
            public string Username { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        { 
            Username = user.UserName;
            Input = new UsernameInputModel
            {
                Username = user.UserName,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeUsernameAsync()
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

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Username != Username)
            {
                user.UserName = Input.Username;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    StatusMessage = "Username was changed";
                    return RedirectToPage();
                }
                else
                {
                    await _signInManager.RefreshSignInAsync(user);
                    ModelState.AddModelError(string.Empty, "Username wasn't changed successfully");
                    return RedirectToPage();

                }
            }
            else 
            {
                await _signInManager.RefreshSignInAsync(user);
                ModelState.AddModelError(string.Empty, "Usernames are identical");
                return RedirectToPage();
            }
        }
    }
}
