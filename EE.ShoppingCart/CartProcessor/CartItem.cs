namespace EE.ShoppingCart.Cart
{
    public class CartItem
    {
        public string ProductName { get; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; }

        public CartItem(string productName, int quantity, decimal unitPrice)
        {
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public decimal Total => Quantity * UnitPrice;
    }
}