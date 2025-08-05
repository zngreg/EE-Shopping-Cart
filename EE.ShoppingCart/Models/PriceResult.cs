namespace EE.ShoppingCart.Models
{
    public record PriceResult(bool Success, decimal Price, string? ErrorMessage);
}