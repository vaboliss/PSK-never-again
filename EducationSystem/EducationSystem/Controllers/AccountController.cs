using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IWorker _workerServices;
        private RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager
            , IWorker workerServices)
        { 
            _userManager = userManager;
            _workerServices = workerServices;
            _roleManager = roleManager;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateNew(string email, string username, string firstName, string lastName)
        {
            var isUsernameExist = await _userManager.FindByNameAsync(username);
            if (isUsernameExist != null)
            {
                TempData["Message"] = "Username is taken";
                return RedirectToAction("Index");
            }

            var isEmailExist = await _userManager.FindByEmailAsync(email);

            if (isEmailExist != null)
            {
                TempData["Message"] = "Email is taken";
                return RedirectToAction("Index");
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
                Email = email,
                WorkerId = id
            };
            string generatedPassword = GeneratePassword(3, 3, 3);
            var createResult = await _userManager.CreateAsync(user, generatedPassword);

            if (createResult.Succeeded)
            {
                var IsRoleExist = await _roleManager.FindByNameAsync("Worker");
                if (IsRoleExist == null)
                {
                    var roleCreated = await _roleManager.CreateAsync(new IdentityRole("Worker"));
                    if (!roleCreated.Succeeded)
                    {
                        TempData["Message"] = "Error creating an role";
                        return RedirectToAction("Index");
                    }
                }
                var AddedRole = await _userManager.AddToRoleAsync(user, "Worker");
                if (AddedRole.Succeeded)
                {
                    MailMessage mail = new MailMessage();

                    mail.From = new MailAddress("educationsystemmail@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Invitation to education system";
                    mail.Body = "These are auto generated credential\n" +
                                $"Username: {username}\n" +
                                $"Password: {generatedPassword}";

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("educationsystemmail@gmail.com", "MailasTestas123");
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }

                    TempData["Message"] = "Invitation sent";
                    return RedirectToAction("Index");
                }
            }

            TempData["Message"] = "Created an user failed";
            return RedirectToAction("Index");

        }
        public string GeneratePassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }
    }
}