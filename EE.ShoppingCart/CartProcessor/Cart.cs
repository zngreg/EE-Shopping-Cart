using EE.ShoppingCart.Cart;
using EE.ShoppingCart.Services;

namespace EE.ShoppingCart.CartProcessor
{
    public class Cart
    {
        private readonly IPriceService _priceService;
        private Dictionary<string, CartItem> _items = new();

        public Cart(IPriceService priceService)
        {
            _priceService = priceService;
        }

        public decimal TaxRate { get; set; } = 0.125M;

        public async Task AddItemAsync(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
                throw new ArgumentException("Product name cannot be empty and quantity must be greater than zero.");

            var price = await _priceService.GetPriceAsync(productName);
            if (price <= 0)
                throw new ArgumentException($"Product '{productName}' is not available.");

            if (_items.TryGetValue(productName, out CartItem? cartItem))
                cartItem.Quantity += quantity;
            else
                _items[productName] = new CartItem(productName, quantity, price);
        }

        public IReadOnlyCollection<CartItem> Items => _items.Values;

        public decimal Subtotal => Math.Round(_items.Values.Sum(i => i.Total), 2);

        public decimal Tax => Math.Round(Subtotal * TaxRate, 2);

        public decimal Total => Subtotal + Tax;
    }
}