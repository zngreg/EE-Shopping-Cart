using System.Text.Json.Serialization;

namespace EE.ShoppingCart.Models
{
    public class ProductPrice
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}