using System.Text.Json;
using EE.ShoppingCart.Models;
using Microsoft.Extensions.Logging;

namespace EE.ShoppingCart.Services
{
    public class PriceService : IPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PriceService> _logger;

        public PriceService(HttpClient httpClient, ILogger<PriceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal> GetPriceAsync(string product)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(product))
                {
                    _logger.LogWarning("Product name cannot be empty.");
                    return 0;
                }

                var url = $"/backend-take-home-test-data/{product}.json";
                var response = await _httpClient.GetStringAsync(url);
                var price = JsonSerializer.Deserialize<ProductPrice>(response);
                return price?.Price ?? 0;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to fetch price for product: {product}. Error: {ex.Message}", product, ex.Message);
                return 0;
            }
        }
    }
}