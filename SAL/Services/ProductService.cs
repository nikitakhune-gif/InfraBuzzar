using InfraBuzzar.DAL.Interfaces;
using InfraBuzzar.Models;

namespace InfraBuzzar.SAL.Services
{
    public class ProductService : Interfaces.IProductService
    {
        public readonly IProductRepository _prodrepository;

        public ProductService(IProductRepository prodrepository)
        {
            _prodrepository = prodrepository;
        }

        public bool AddProduct(Product product)
        {
            Boolean status = _prodrepository.AddProduct(product);
            return status;
        }

        public List<Product> getProducts()
        {
            List<Product> prodlist = _prodrepository.getAllProducts();
            return prodlist;
        }
    }
}
