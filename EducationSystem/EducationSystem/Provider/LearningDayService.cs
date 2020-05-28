using EducationSystem.Data;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace EducationSystem.Provider
{
    public class LearningDayService : ILearningDay
    {
        private readonly EducationSystemDbContext _edu;
        private readonly UserManager<ApplicationUser> _userManager;

        public LearningDayService(EducationSystemDbContext edu, UserManager<ApplicationUser> userManager)
        {
            _edu = edu;
            _userManager = userManager;
        }

        // Returns all learning days
        public List<LearningDay> GetAllLearningDays()
        {
            List<LearningDay> learningDays = _edu.LearningDays.Include(ld => ld.Topic).ToList();
            return learningDays;
        }

        // Returns all learning days by specified worker
        public List<LearningDay> GetLearningDaysByWorker(Worker worker)
        {
            List<LearningDay> learningDays = _edu.LearningDays.Where(ld => ld.WorkerId == worker.Id).Include(ld => ld.Topic).ToList();
            return learningDays;
        }
        public void SendMail(LearningDay learningDay)
        {

            MailMessage mail = new MailMessage();
            var user = _userManager.Users.FirstOrDefault(u => u.WorkerId == learningDay.WorkerId);
            
            mail.From = new MailAddress("educationsystemmail@gmail.com");
            mail.To.Add(user.Email);
            mail.Subject = "You have new learning day";
            mail.Body = "Your learning day information :\n" +
                        $"Date: {learningDay.Date.ToString("yyyy/MM/dd")}\n" +
                        $"Topic: {learningDay.Topic.Name}\n" +
                        $"Description: {learningDay.Topic.Description}";

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("educationsystemmail@gmail.com", "MailasTestas123");
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);
            }

        }
    }
}
