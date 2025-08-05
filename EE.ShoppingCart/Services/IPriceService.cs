using EE.ShoppingCart.Models;

namespace EE.ShoppingCart.Services
{
    public interface IPriceService
    {
        Task<PriceResult> GetPriceAsync(string productName);
    }
}