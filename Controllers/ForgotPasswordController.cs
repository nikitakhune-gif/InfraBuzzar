using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using InfraBuzzar.Models;
using Microsoft.AspNetCore.Http;
using InfraBuzzar.Data;
using InfraBuzzar.SAL.Interfaces;

namespace Infra_Mart.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IUserService userInterface;

        public ForgotPasswordController(ApplicationDbContext db, IUserService userInterface)
        {
            this.db = db;
            this.userInterface = userInterface;
        }

        public IActionResult ForgotPass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPass(string email)
        {
            Random rnd = new Random();
            int otp = rnd.Next(100000, 999999);

            User user = FindByEmail(email);
            if (user == null)
            {
                TempData["Error"] = "Email not found!";
                return View();
            }

            HttpContext.Session.SetInt32("OTP", otp);
            HttpContext.Session.SetObject("forgotpassuser", user);

            // Send Mail
            string tomail = user.Email;
            string fromMail = "inframartonline@gmail.com";
            string fromPassword = "afnzxmohndggfazb";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "OTP From InfraMart";
            message.To.Add(new MailAddress(tomail));
            message.Body = "Your OTP is " + otp;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);

            return RedirectToAction("VerifyOtp");
        }

        public IActionResult VerifyOtp()
        {
            return View();
        }

        //public IActionResult UpdatePassword(string otp)
        //{
        //    int otp1 = Convert.ToInt32(otp);
        //    int otp2 = Convert.ToInt32(HttpContext.Session.GetInt32("OTP"));

        //    User user = HttpContext.Session.GetObject<User>("forgotpassuser");

        //    if (otp1 == otp2)
        //    {
        //        ViewData["user"] = user;
        //        return View();
        //    }

        //    TempData["Error"] = "Invalid OTP!";
        //    return RedirectToAction("VerifyOtp");


        //}

        public IActionResult UpdatePass(string otp)
        {
            int otp1 = Convert.ToInt32(otp);
            int otp2 = Convert.ToInt32(HttpContext.Session.GetInt32("OTP"));

            User user = HttpContext.Session.GetObject<User>("forgotpassuser");

            if (user == null)
            {
                TempData["Error"] = "Session expired! Try again.";
                return RedirectToAction("ForgotPassword");
            }

            if (otp1 == otp2)
            {
                ViewData["user"] = user;
                return View();
            }

            TempData["Error"] = "Invalid OTP!";
            return RedirectToAction("VerifyOtp");
        }

        [HttpPost]
        public string ChangePassword([FromBody] User user)
        {
            User olduser = HttpContext.Session.GetObject<User>("forgotpassuser");

            olduser.Password = userInterface.ConvertToEncrypt(user.Password);
            db.User.Update(olduser);
            db.SaveChanges();

            return "Password updated successfully";

            /*User olduser = HttpContext.Session.GetObject<User>("forgotpassuser");
            olduser.Password = ConvertToEncrypt(user.Password);

            db.User.Update(olduser);
            db.SaveChanges();

            return "Password updated successfully!";*/
        }

        public User FindByEmail(string email)
        {
            return db.User.FirstOrDefault(u => u.Email == email);
        }
    }
}
