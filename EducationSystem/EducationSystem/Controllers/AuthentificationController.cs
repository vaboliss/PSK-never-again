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

        private SignInManager<ApplicationUser> _signInManager;
        public Authentification(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager
            ,IWorker workerServices)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _workerServices = workerServices;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        { 
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

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

            var user = new ApplicationUser()
            {
                UserName = username
            };
            
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                Worker worker = new Worker()
                {
                    FirstName = firstName,
                    LastName = lastName
                };
                int id = _workerServices.CreateWorkerRId(worker);

                user.workerId = id;

                var result1 = _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                    if (signInResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                    else
                    {
                        //failed to register
                        return RedirectToAction("Register");
                    }
                }
                else
                {
                    var result2 = _userManager.DeleteAsync(user);
                    _workerServices.RemoveWorkerById(id);

                }
            }
            //failed to register
            return RedirectToAction("Index", "Authentification");
        }

        public async Task<IActionResult> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult ForgotPassword(string email)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
