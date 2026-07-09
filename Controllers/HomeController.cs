using System.Diagnostics;
using InfraBuzzar.Data;
using InfraBuzzar.Models;
using InfraBuzzar.SAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfraBuzzar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IUserService userInterface;

        public HomeController(ApplicationDbContext db, IUserService userInterface)

        {
            this.db = db;
            this.userInterface = userInterface;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult About()
        {

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {

            Console.WriteLine("method Called");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string identifier, string password)
        {

            Console.WriteLine(identifier);
            Console.WriteLine(password);
            List<User> ulist = userInterface.getAllUser();
            string pass = userInterface.ConvertToEncrypt(password);
            foreach (User u in ulist)
            {

                // Allow login using either email or username (FirstName)
                bool identifierMatches = false;
                if (!string.IsNullOrWhiteSpace(identifier))
                {
                    identifierMatches = string.Equals(u.Email?.Trim(), identifier.Trim(), StringComparison.OrdinalIgnoreCase)
                                        || string.Equals(u.FirstName?.Trim(), identifier.Trim(), StringComparison.OrdinalIgnoreCase);
                }

                if (identifierMatches && u.Password.Equals(pass))
                {
                    //HttpContext.Session.GetObject<User>("LoginUser", u);
                    HttpContext.Session.SetObject("LoginUser", u);

                    if (u.Role.Equals("admin"))
                    {
                        return RedirectToAction("Admin", "Admin");
                    }
                    else if (u.Role.Equals("manager"))
                    {
                        return RedirectToAction("Manager", "Admin");
                    }
                    return RedirectToAction("ViewAll", "Product");

                }


            }
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {

            Boolean status = userInterface.addNewUser(user);
            Console.WriteLine(status);
            if (status)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return RedirectToAction("Home", "about");
            }

        }
    }
  }

