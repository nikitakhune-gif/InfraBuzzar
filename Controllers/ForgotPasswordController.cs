using Microsoft.AspNetCore.Mvc;
using MimeKit;
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

        [HttpGet]
        public IActionResult ForgotPass()
        {
            return View();
        }

        // FIX: added [FromBody] so the "email" string sent as JSON from the
        // frontend (axios.post with JSON.stringify(email)) actually binds.
        // Before this fix "email" always came in as null on the server,
        // FindByEmail(null) returned null, and the method returned BEFORE
        // ever reaching the mail-send code below -- this is exactly why no
        // OTP mail was arriving, with no visible error.
        [HttpPost]
        public IActionResult ForgotPass([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Email is required." });
            }

            User user = FindByEmail(email);
            if (user == null)
            {
                return Json(new { success = false, message = "Email not found!" });
            }

            Random rnd = new Random();
            int otp = rnd.Next(100000, 999999);

            HttpContext.Session.SetInt32("OTP", otp);
            HttpContext.Session.SetObject("forgotpassuser", user);

            try
            {
                SendOtpMail(user.Email, otp);
            }
            catch (Exception ex)
            {
                // Mail failed to send (bad app-password, port blocked, etc.)
                // Returned as JSON instead of failing silently.
                return Json(new { success = false, message = "Could not send OTP mail: " + ex.Message });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        // FIX: renamed from "UpdatePass" to "UpdatePassword" so the URL the
        // frontend actually calls (/ForgotPassword/UpdatePassword) resolves.
        // This name mismatch was the exact 404 seen in the console.
        // FIX: now returns Json instead of rendering a View. An AJAX call
        // can't meaningfully consume a redirected View's HTML -- the old
        // code looked "successful" to the JS even when the OTP was wrong.
        [HttpPost]
        public IActionResult UpdatePassword([FromBody] string otp)
        {
            int? storedOtp = HttpContext.Session.GetInt32("OTP");
            User user = HttpContext.Session.GetObject<User>("forgotpassuser");

            if (user == null || storedOtp == null)
            {
                return Json(new { success = false, message = "Session expired! Please try again." });
            }

            if (!int.TryParse(otp, out int enteredOtp) || enteredOtp != storedOtp.Value)
            {
                return Json(new { success = false, message = "Invalid OTP!" });
            }

            return Json(new { success = true });
        }

        // Separate GET action: renders the "set new password" page once the
        // OTP has already been verified and is sitting in session.
        [HttpGet]
        public IActionResult UpdatePassword()
        {
            User user = HttpContext.Session.GetObject<User>("forgotpassuser");
            if (user == null)
            {
                TempData["Error"] = "Session expired! Try again.";
                // FIX: was RedirectToAction("ForgotPassword") - no such
                // action exists, only "ForgotPass" does.
                return RedirectToAction("ForgotPass");
            }

            ViewData["user"] = user;
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword([FromBody] User user)
        {
            User olduser = HttpContext.Session.GetObject<User>("forgotpassuser");

            // FIX: null-guard. Old code would throw a NullReferenceException
            // here if the session had already expired.
            if (olduser == null)
            {
                return Json(new { success = false, message = "Session expired! Try again." });
            }

            olduser.Password = userInterface.ConvertToEncrypt(user.Password);
            db.User.Update(olduser);
            db.SaveChanges();

            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("forgotpassuser");

            return Json(new { success = true, message = "Password updated successfully" });
        }

        private void SendOtpMail(string toMail, int otp)
        {
            // TODO: move these to appsettings.json / user-secrets instead of
            // hardcoding credentials in source code.
            string fromMail = "nikitakhune95@gmail.com";
            string fromPassword = "ekdlzsvgfohivriw"; // Gmail App Password (16 chars, no spaces)

            // FIX: guard against invisible whitespace / wrong-length app
            // passwords copy-pasted from Google's UI -- a stray space or
            // truncated copy causes the exact same 535 BadCredentials error
            // as a genuinely wrong password, with no way to tell them apart
            // from the error message alone.
            fromPassword = fromPassword?.Trim();
            if (string.IsNullOrEmpty(fromPassword) || fromPassword.Length != 16)
            {
                throw new Exception("App password missing or invalid length (expected 16 characters, no spaces).");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("InfraMart", fromMail));
            message.To.Add(MailboxAddress.Parse(toMail));
            message.Subject = "OTP From InfraMart";
            message.Body = new TextPart("html")
            {
                Text = "Your OTP is " + otp
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

            // FIX: some environments cause MailKit to attempt XOAUTH2 first,
            // which fails immediately for a plain app-password login and can
            // surface as a misleading BadCredentials error. Forcing plain
            // AUTH ensures the app password is actually what gets tried.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            client.Authenticate(fromMail, fromPassword);
            client.Send(message);
            client.Disconnect(true);
        }

        public User FindByEmail(string email)
        {
            return db.User.FirstOrDefault(u => u.Email == email);
        }
    }
}
