using InfraBuzzar.Models;

namespace InfraBuzzar.SAL.Interfaces
{
    public interface ICategoryService
    {
        bool addNewCategory(string categoryName);
        bool deleteCategoriy(int id);
        List<Category> getAllCategories();
    }
}
