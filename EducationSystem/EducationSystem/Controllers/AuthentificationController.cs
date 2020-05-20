using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWorker _workerServices;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<ApplicationUser> _signInManager;
        public Authentification(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager
            ,IWorker workerServices)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _workerServices = workerServices;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        { 
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index");

          
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(string username, string firstName, string lastName, string password, string repeatedPassword)
        {
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
                     return RedirectToAction("Index", "Home");
                }
            }
            //failed to register
            return RedirectToAction("Index", "Authentification");
        }
        [HttpPost]
        public async Task<IActionResult> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
