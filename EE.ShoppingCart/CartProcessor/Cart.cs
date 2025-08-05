using EE.ShoppingCart.Cart;
using EE.ShoppingCart.Services;

namespace EE.ShoppingCart.CartProcessor
{
    public class Cart
    {
        private readonly IPriceService _priceService;
        private Dictionary<string, CartItem> _items = [];

        public Cart(IPriceService priceService)
        {
            _priceService = priceService;
        }

        public decimal TaxRate { get; set; } = 0.125M;

        public async Task AddItemAsync(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
                throw new ArgumentException("Product name cannot be empty and quantity must be greater than zero.");

            var priceResult = await _priceService.GetPriceAsync(productName);
            if (!priceResult.Success || !string.IsNullOrWhiteSpace(priceResult.ErrorMessage))
                throw new InvalidOperationException(priceResult.ErrorMessage);

            if (_items.TryGetValue(productName, out CartItem? cartItem))
                cartItem.Quantity += quantity;
            else
                _items[productName] = new CartItem(productName, quantity, priceResult.Price);
        }

        public async Task<bool> RemoveItemAsync(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
                throw new ArgumentException("Product name cannot be empty and quantity must be greater than zero.");

            if (_items.TryGetValue(productName, out CartItem? cartItem))
            {
                if (cartItem.Quantity < quantity)
                    // Setting the quantity to zero to ensure we don't remove more than item's quantity.
                    cartItem.Quantity = 0;
                else
                    cartItem.Quantity -= quantity;

                if (cartItem.Quantity == 0)
                    _items.Remove(productName);

                return true;
            }

            return false;
        }

        public IReadOnlyCollection<CartItem> Items => _items.Values;

        public decimal Subtotal => Math.Round(_items.Values.Sum(i => i.Total), 2);

        public decimal Tax => Math.Round(Subtotal * TaxRate, 2);

        public decimal Total => Subtotal + Tax;
    }
}