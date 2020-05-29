using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EducationSystem.Models;
using Microsoft.AspNetCore.Identity;
using EducationSystem.Interfaces;

namespace EducationSystem.Controllers
{
    public class Authentification : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IWorker _workerServices;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<ApplicationUser> _signInManager;
        public Authentification(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager
            ,IWorker workerServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _workerServices = workerServices;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index","Calendar");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(string username, string firstName, string lastName, string password, string repeatedPassword)
        {
            var userdd = HttpContext.User;
            if (password != repeatedPassword)
            {
                //failed to register
                return RedirectToAction("Index", "Authentification");
            }

            var isUsernameExist = await _userManager.FindByNameAsync(username);
            if (isUsernameExist != null)
            {
                //failed to register
                return RedirectToAction("Index", "Authentification");
            }

            Worker worker = new Worker()
            {
                FirstName = firstName,
                LastName = lastName
            };
            int id = _workerServices.CreateWorkerRId(worker);
            var user = new ApplicationUser()
            {
                UserName = username,
                WorkerId = id
            };

            var createResult = await _userManager.CreateAsync(user, password);

            if (createResult.Succeeded)
            {
                var IsRoleExist = await _roleManager.FindByNameAsync("Worker");
                if (IsRoleExist == null)
                {
                    var roleCreated = await _roleManager.CreateAsync(new IdentityRole("Worker"));
                    if (!roleCreated.Succeeded)
                    {
                        return RedirectToAction("Index", "Authentification");
                    }
                }
                var AddedRole = await _userManager.AddToRoleAsync(user, "Worker");
                if (AddedRole.Succeeded)
                {
                     return RedirectToAction("Index", "Calendar");
                }
            }
            //failed to register
            return RedirectToAction("Index", "Authentification");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
