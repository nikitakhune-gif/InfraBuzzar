using InfraBuzzar.Data;
using InfraBuzzar.Models;

namespace InfraBuzzar.DAL.Repository
{
    public class ProductRepository : Interfaces.IProductRepository
    {

        public readonly ApplicationDbContext _ctx;
        public ProductRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public bool AddProduct(Product product)
        {
            _ctx.Product.Add(product);
            _ctx.SaveChanges();
            return true;
        }

        public List<Product> getAllProducts()
        {

            //CollectionContex ctx = new CollectionContex();


            var products = from product in _ctx.Product select product;
            return products.ToList<Product>();
        }

    }
}