namespace InfraBuzzar.View_Models
{
    public class CartItemViewModel
    {

        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }

        public decimal Total => ProductPrice * Quantity;
    }
}
