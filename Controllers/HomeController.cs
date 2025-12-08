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
        public IActionResult Login(string email, string password)
        {

            Console.WriteLine(email);
            Console.WriteLine(password);
            List<User> ulist = userInterface.getAllUser();
            string pass = userInterface.ConvertToEncrypt(password);
            foreach (User u in ulist)
            {

                if (u.Email.Equals(email) && u.Password.Equals(pass))
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

