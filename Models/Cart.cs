using System.ComponentModel.DataAnnotations;

namespace InfraBuzzar.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public long UserId { get; set; }

        public int ProductId { get; set; }
    }
}
