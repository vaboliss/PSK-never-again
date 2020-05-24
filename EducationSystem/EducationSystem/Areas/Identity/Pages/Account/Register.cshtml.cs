using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EducationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EducationSystem.Interfaces;

namespace EducationSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IWorker _workerService;


        public RegisterModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            IWorker workerService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _workerService = workerService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = new ApplicationUser();
            user.Email = Input.Email;
            user.UserName = Input.Username;

            var worker = new Worker()
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName
            };

            var workerId = _workerService.CreateWorkerRId(worker);

            user.WorkerId = workerId;

            var result = await _userManager.CreateAsync(user,Input.Password);
            if (result.Succeeded)
            {
                ErrorMessage = "User successfully created";
                return RedirectToPage();
            }
            else
            {
                ErrorMessage = "Something went wrong";
                return RedirectToPage();
            }
            return Page();
        }
    }
}
