using System.ComponentModel.DataAnnotations;

namespace InfraBuzzar.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int ProductId { get; set; }

        // Use long to match User.UserId (bigint in database)
        public long UserId { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
