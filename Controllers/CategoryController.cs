using InfraBuzzar.Data;
using InfraBuzzar.Models;
using Microsoft.AspNetCore.Mvc;

namespace InfraBuzzar.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(Category cat)
        {
            _db.Category.Add(cat);
            _db.SaveChanges();
            return RedirectToAction("ShowAllCategory");
        }

        //[HttpPost]
        //public IActionResult CreateCategory(Category cat)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(cat);
        //    }

        //    _db.Category.Add(cat);
        //    _db.SaveChanges();

        //    return RedirectToAction("ShowAllCategory");
        //}


        public IActionResult ShowAllCategory()
        {
            //var list = _db.Category.ToList();
            //return View(list);

            var list = _db.Category.ToList();

            if (list == null)
                list = new List<Category>();  // Safety check

            return View(list);
        }

        public IActionResult DeleteCategory(int id)
        {
            var cat = _db.Category.Find(id);
            if (cat != null)
            {
                _db.Category.Remove(cat);
                _db.SaveChanges();
            }

            return RedirectToAction("ShowAllCategory");
        }
    }
}
