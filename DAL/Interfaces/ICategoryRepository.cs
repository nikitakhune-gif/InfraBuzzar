using InfraBuzzar.Models;

namespace InfraBuzzar.DAL.Interfaces
{
    public interface ICategoryRepository
    {
        bool addCategory(string categoryName);
        bool deleteCategory(int id);
        List<Category> getCategories();
    }
}
