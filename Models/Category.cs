using System.ComponentModel.DataAnnotations;

namespace InfraBuzzar.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        // One-to-many relation (Category → Products)
        public List<Product> Products { get; set; }
    }
}
