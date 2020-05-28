using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EducationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Data.Odbc;
using System.Data;

namespace EducationSystem.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender
            , IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
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

        public async Task<IActionResult> OnPostChangeEmailAsync()
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

            string conStr = _configuration.GetConnectionString("EducationSystemDbContext");
            var query = "Update dbo.AspNetUsers " +
                         "Set  Email = @InputEmail, NormalizedEmail = @InputNormalized " +
                         "WHERE UserName = @Username";

            using (SqlConnection sqlConnection = new SqlConnection(conStr))
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {

                command.Parameters.Add("@InputEmail", SqlDbType.NVarChar, 250);
                command.Parameters["@InputEmail"].Value = Input.NewEmail;
                command.Parameters.Add("@InputNormalized", SqlDbType.NVarChar, 250);
                command.Parameters["@InputNormalized"].Value = Input.NewEmail.ToUpper();
                command.Parameters.Add("@Username", SqlDbType.NVarChar, 250);
                command.Parameters["@Username"].Value = user.UserName;

                try
                {
                    var query1 = command.CommandText;
                    sqlConnection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        ModelState.AddModelError(string.Empty, "Email wasn't changed successfully");
                        return RedirectToPage();
                    }

                    await _signInManager.RefreshSignInAsync(user);
                    StatusMessage = "Email was changed";
                    return RedirectToPage();

                }
                catch (Exception ex)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    ModelState.AddModelError(string.Empty, "Email wasn't changed successfully");
                    return RedirectToPage();
                }

            }
        }
    }
}
