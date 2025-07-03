using EE.ShoppingCart.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace EE.ShoppingCart.Unit.Tests
{
    [TestFixture]
    public class PriceServiceTests
    {
        [Test]
        public async Task GetPriceAsync_With_ValidProduct_ReturnsCorrectPrice()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://equalexperts.github.io")
            };

            var service = new PriceService(httpClient, Mock.Of<ILogger<PriceService>>());
            var price = await service.GetPriceAsync("cornflakes");
            Assert.That(price, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetPriceAsync_With_InvalidProduct_ReturnsZero()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://equalexperts.github.io")
            };

            var service = new PriceService(httpClient, Mock.Of<ILogger<PriceService>>());
            var price = await service.GetPriceAsync("nonExistentProduct");
            Assert.That(price, Is.EqualTo(0));
        }

        [Test]
        public async Task GetPriceAsync_With_EmptyProductName_ReturnsZero()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://equalexperts.github.io")
            };

            var service = new PriceService(httpClient, Mock.Of<ILogger<PriceService>>());
            var price = await service.GetPriceAsync(string.Empty);
            Assert.That(price, Is.EqualTo(0));
        }
    }
}