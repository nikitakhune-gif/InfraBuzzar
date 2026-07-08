using InfraBuzzar.Data;
using InfraBuzzar.Models;
using InfraBuzzar.View_Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
        // Accepts a JSON array of product IDs from the client. Uses session to get logged-in user.
        [HttpPost]
        public IActionResult AddToCart([FromBody] List<int> productIds)
        {
            var user = HttpContext.Session.GetObject<User>("LoginUser");
            if (user == null)
            {
                // Caller will handle 401 and prompt login
                return Unauthorized();
            }

            if (productIds == null || productIds.Count == 0)
            {
                return BadRequest();
            }

            long uid = user.UserId;

            foreach (var productId in productIds)
            {
                var existing = _db.Cart.FirstOrDefault(c => c.ProductId == productId && c.UserId == uid);
                if (existing != null)
                {
                    existing.Quantity += 1;
                }
                else
                {
                    Cart cart = new Cart()
                    {
                        ProductId = productId,
                        UserId = uid,
                        Quantity = 1
                    };
                    _db.Cart.Add(cart);
                }
            }

            _db.SaveChanges();

            return Ok(new { success = true });
        }

        // SHOW CART
        // Joins Cart rows for this user with Product details.
        public IActionResult ShowCart(int? userId)
        {
            // If no userId provided, use logged-in session user
            if (!userId.HasValue)
            {
                var sessionUser = HttpContext.Session.GetObject<User>("LoginUser");
                if (sessionUser == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                userId = (int)sessionUser.UserId;
            }

            var cartItems = (from c in _db.Cart
                             join p in _db.Product on c.ProductId equals p.ProductId
                             where c.UserId == userId.Value
                             select new CartItemViewModel
                             {
                                 CartId = c.CartId,
                                 ProductId = p.ProductId,
                                 ProductName = p.ProductName,
                                 ProductPrice = (decimal)p.ProductPrice,
                                 ImageUrl = p.ProductImage,
                                 Quantity = c.Quantity
                             }).ToList();

            ViewBag.UserId = userId;
            return View(cartItems);
        }

        // UPDATE QUANTITY (called via fetch from the cart page)
        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateQtyRequest req)
        {
            var cartItem = _db.Cart.FirstOrDefault(c => c.CartId == req.CartId);
            if (cartItem == null) return NotFound();

            cartItem.Quantity = req.Quantity < 1 ? 1 : req.Quantity;
            _db.SaveChanges();

            return Ok();
        }

        // REMOVE FROM CART
        [HttpPost]
        public IActionResult RemoveFromCart(int cartId)
        {
            var cartItem = _db.Cart.FirstOrDefault(c => c.CartId == cartId);
            if (cartItem == null) return NotFound();

            _db.Cart.Remove(cartItem);
            _db.SaveChanges();

            return Ok();
        }

        // CHECKOUT PAGE
        // Shows order summary + shipping/billing form before placing the order.
        public IActionResult Checkout()
        {
            var sessionUser = HttpContext.Session.GetObject<User>("LoginUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = (int)sessionUser.UserId;

            var cartItems = (from c in _db.Cart
                             join p in _db.Product on c.ProductId equals p.ProductId
                             where c.UserId == userId
                             select new CartItemViewModel
                             {
                                 CartId = c.CartId,
                                 ProductId = p.ProductId,
                                 ProductName = p.ProductName,
                                 ProductPrice = (decimal)p.ProductPrice,
                                 ImageUrl = p.ProductImage,
                                 Quantity = c.Quantity
                             }).ToList();

            // No point showing checkout if the cart is empty
            if (!cartItems.Any())
            {
                return RedirectToAction("ShowCart");
            }

            // Pre-fill name/phone if you store them on the User model
            // User model has FirstName and LastName fields — use them for display
            ViewBag.FullName = string.Concat(sessionUser.FirstName, " ", sessionUser.LastName);

            return View(cartItems);
        }

        // PLACE ORDER
        // Called when the checkout form is submitted. Replace the TODO with
        // your actual Orders table logic if/when you add one.
        [HttpPost]
        public IActionResult PlaceOrder(string fullName, string phone, string address, string city, string pincode)
        {
            var sessionUser = HttpContext.Session.GetObject<User>("LoginUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = (int)sessionUser.UserId;

            var cartItems = _db.Cart.Where(c => c.UserId == userId).ToList();
            if (!cartItems.Any())
            {
                return RedirectToAction("ShowCart");
            }

            // TODO: Persist the order (Orders / OrderItems tables) here using
            // fullName, phone, address, city, pincode and the cartItems above.

            // Empty the cart now that the order is "placed"
            _db.Cart.RemoveRange(cartItems);
            _db.SaveChanges();

            return RedirectToAction("OrderConfirmation");
        }

        // ORDER CONFIRMATION
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }

    public class UpdateQtyRequest
    {
        public int CartId { get; set; }
        public int Quantity { get; set; }
    }
}