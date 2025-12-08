using InfraBuzzar.Models;

namespace InfraBuzzar.SAL.Interfaces
{
    public interface IProductService
    {
        bool AddProduct(Product product);
        List<Product> getProducts();
    }
}
