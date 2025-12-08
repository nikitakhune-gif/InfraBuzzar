using InfraBuzzar.Data;
using InfraBuzzar.Models;
using InfraBuzzar.SAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfraBuzzar.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext ctx;
        private readonly IUserService userInterface;

        public AdminController(ApplicationDbContext ctx, IUserService userInterface)
        {
            this.ctx = ctx;
            this.userInterface = userInterface;
        }
        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult Manager()
        {
            return View();
        }

        public IActionResult AddUser()
        {
            return View();
        }

        public IActionResult ViewAllAdmin()
        {
            var products = ctx.Product.ToList();
            ViewData["products"] = products;
            return View();
        }


        [HttpPost]
        public string AddUser([FromBody] User user)
        {
            User newUser = new User();
            newUser = user;
            newUser.Password = userInterface.ConvertToEncrypt(user.Password);
            newUser.RegistrationDate = DateTime.Now.Date;
            ctx.User.Add(newUser);
            int s = ctx.SaveChanges();
            Console.WriteLine("value of s is" + s);


            return "user Added";
        }

        public IActionResult ShowAllUser()
        {
            List<User> userlist = ctx.User.ToList();
            ViewData["userlist"] = userlist;
            return View();
        }

        [HttpPost]
        public string DeleteUser([FromBody] User user)
        {
            Console.WriteLine(user.UserId);
            List<User> userlist = ctx.User.ToList();
            foreach (User u in userlist)
            {
                if (u.UserId == user.UserId)
                {
                    Console.WriteLine(u);
                    ctx.User.Remove(u);
                    ctx.SaveChanges();

                    return "user-delete";
                }
            }
            return "Not-Found";

        }

        [HttpPost]
        public string RemovePrdouct([FromBody] Product prod)
        {
            Console.WriteLine("Method Called");
            Console.WriteLine(prod.ProductId);
            List<Product> prodcuts = ctx.Product.ToList();

            foreach (Product p in prodcuts)
                if (p.ProductId == prod.ProductId)
                {
                    ctx.Product.Remove(p);
                    ctx.SaveChanges();
                }
            return "Product-Delete";

        }
    }
}


