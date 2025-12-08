using InfraBuzzar.Models;

namespace InfraBuzzar.DAL.Interfaces
{
    public interface IProductRepository
    {
        bool AddProduct(Product product);
        List<Product> getAllProducts();
    }
}
