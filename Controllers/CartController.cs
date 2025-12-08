using InfraBuzzar.Data;
using InfraBuzzar.Models;
using Microsoft.AspNetCore.Mvc;

namespace InfraBuzzar.Controllers
{
    public class CartController : Controller
    {

        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ADD TO CART
        public IActionResult AddToCart(int productId, int userId)
        {
            Cart cart = new Cart()
            {
                ProductId = productId,
                UserId = userId
            };

            _db.Cart.Add(cart);
            _db.SaveChanges();

            return RedirectToAction("ShowCart", new { userId = userId });
        }

        // SHOW CART
        public IActionResult ShowCart(int userId)
        {
            //var cartItems = _db.Cart.Where(c => c.UserId == userId).ToList();


            //return View(cartItems);
            List<Product> cartProducts = _db.Product.ToList();
            return View(cartProducts);
        }
    }
}

