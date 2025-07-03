namespace EE.ShoppingCart.Services
{
    public interface IPriceService
    {
        Task<decimal> GetPriceAsync(string productName);
    }
}