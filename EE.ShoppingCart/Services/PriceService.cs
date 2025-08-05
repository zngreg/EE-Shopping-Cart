using System.Net.Http.Json;
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

        public async Task<PriceResult> GetPriceAsync(string product)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(product))
                {
                    _logger.LogWarning("Product name cannot be empty.");
                    return new PriceResult(false, 0, "Product name cannot be empty.");
                }

                var url = $"/backend-take-home-test-data/{product}.json";
                var response = await _httpClient.GetFromJsonAsync<ProductPrice>(url);

                return response != null && !string.IsNullOrWhiteSpace(response.Title)
                    ? new PriceResult(true, response.Price, null)
                    : new PriceResult(false, 0, "Product not found.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to fetch price for product: {product}. Error: {ex.Message}", product, ex.Message);
                return new PriceResult(false, 0, "Failed to fetch price.");
            }
        }
    }
}