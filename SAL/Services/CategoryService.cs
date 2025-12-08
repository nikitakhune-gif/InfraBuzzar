using InfraBuzzar.DAL.Interfaces;
using InfraBuzzar.Models;

namespace InfraBuzzar.SAL.Services
{
    public class CategoryService : Interfaces.ICategoryService
    {

        public readonly ICategoryRepository _categoryrepository;
        public CategoryService(ICategoryRepository categoryrepository)
        {
            _categoryrepository = categoryrepository;
        }

        public bool addNewCategory(string categoryName)
        {
            Boolean status = _categoryrepository.addCategory(categoryName);
            return status;
        }

        public bool deleteCategoriy(int id)
        {
            Boolean status = _categoryrepository.deleteCategory(id);
            return status;
        }

        public List<Category> getAllCategories()
        {
            List<Category> catlist = _categoryrepository.getCategories();
            return catlist;
        }
    }
}
