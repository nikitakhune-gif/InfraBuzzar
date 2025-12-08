using InfraBuzzar.Data;
using InfraBuzzar.Models;
using InfraBuzzar.SAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfraBuzzar.Controllers
{
    //public class ProductController : Controller
    //{

    //    private readonly ApplicationDbContext _db;
    //    private readonly IWebHostEnvironment _env;

    //    public ProductController(ApplicationDbContext db, IWebHostEnvironment env)
    //    {
    //        _db = db;
    //        _env = env;
    //    }

    //    public IActionResult ViewAll()
    //    {
    //        return View(_db.Product.ToList());
    //    }

    //    public IActionResult AddProduct()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public IActionResult AddProduct(Product product, IFormFile imageFile)
    //    {
    //        if (imageFile != null)
    //        {
    //            string folder = Path.Combine(_env.WebRootPath, "Images");
    //            string filename = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

    //            string filePath = Path.Combine(folder, filename);
    //            using var stream = new FileStream(filePath, FileMode.Create);
    //            imageFile.CopyTo(stream);

    //            product.ProductImage = filename;
    //        }

    //        _db.Product.Add(product);
    //        _db.SaveChanges();

    //        return RedirectToAction("ViewAll");
    //    }
    //}

    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _prodservice;
        private readonly ApplicationDbContext ctx;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICategoryService _categoryService123;

        public ProductController(IProductService prodservice, ApplicationDbContext ctx, IWebHostEnvironment webHostEnvironment, ICategoryService categoryService123)
        {
            _prodservice = prodservice;
            this.ctx = ctx;
            _webHostEnvironment = webHostEnvironment;
            _categoryService123 = categoryService123;
        }


        // GET: Product
        public ActionResult ViewAll()
        {
            var products = ctx.Product.ToList();
            ViewData["products"] = products;
            return View();
        }

        // GET: Product/Details/5
        /*public ActionResult Details(int id)
        {
            return View();
        }*/
        public IActionResult AddProduct()
        {

            var categories = _categoryService123.getAllCategories();
            ViewData["Categories"] = categories;
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(FileUpload imagefile)
        {
            Console.WriteLine(imagefile.CategoryId);
            // Product product = JsonConvert.DeserializeObject<Product>(fileobj.Product);
            string file = uploadFile(imagefile);
            var product = new Product()
            {
                ProductName = imagefile.ProductName,
                ProductDescription = imagefile.ProductDescription,
                ProductPrice = imagefile.ProductPrice,
                ProductUnit = imagefile.ProductUnit,
                CategoryId = imagefile.CategoryId,
                ProductImage = file
            };
            ctx.Product.Add(product);
            ctx.SaveChanges();
            return RedirectToAction("ViewAllAdmin", "Admin");
        }

        private string uploadFile(FileUpload imagefile)
        {
            string filenew = null;
            if (imagefile != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                filenew = Guid.NewGuid().ToString() + "-" + imagefile.Image.FileName;
                string filepath = Path.Combine(uploadDir, filenew);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    imagefile.Image.CopyTo(fileStream);
                }
            }
            return filenew;
        }
    }
}


