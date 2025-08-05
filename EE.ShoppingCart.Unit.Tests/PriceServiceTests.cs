using System.Net;
using System.Text.Json;
using EE.ShoppingCart.Models;
using EE.ShoppingCart.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace EE.ShoppingCart.Unit.Tests
{
    [TestFixture]
    public class PriceServiceTests
    {
        private Mock<ILogger<PriceService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<PriceService>>();
        }

        [Test]
        public async Task GetPriceAsync_With_ValidProduct_ReturnsCorrectPrice()
        {
            // Arrange
            var expectedPrice = new ProductPrice { Title = "Cornflakes", Price = 2.52M };
            var json = JsonSerializer.Serialize(expectedPrice);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var service = new PriceService(GetMockHttpClient(response), _loggerMock.Object);

            // Act
            var price = await service.GetPriceAsync("cornflakes");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(price.Success, Is.EqualTo(true));
                Assert.That(price.Price, Is.EqualTo(expectedPrice.Price));
                Assert.That(price.ErrorMessage, Is.EqualTo(null));
            });
        }

        [Test]
        public async Task GetPriceAsync_With_InvalidProduct_ReturnsZero()
        {
            // Arrange
            ProductPrice expectedPrice = new();
            var json = JsonSerializer.Serialize(expectedPrice);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var service = new PriceService(GetMockHttpClient(response), _loggerMock.Object);

            // Act
            var price = await service.GetPriceAsync("nonExistentProduct");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(price.Success, Is.EqualTo(false));
                Assert.That(price.Price, Is.EqualTo(expectedPrice.Price));
                Assert.That(price.ErrorMessage, Is.EqualTo("Product not found."));
            });

        }

        [Test]
        public async Task GetPriceAsync_With_EmptyProductName_ReturnsZero()
        {
            // Arrange
            ProductPrice expectedPrice = new();
            var json = JsonSerializer.Serialize(expectedPrice);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var service = new PriceService(GetMockHttpClient(response), _loggerMock.Object);

            // Act
            var price = await service.GetPriceAsync(string.Empty);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(price.Success, Is.EqualTo(false));
                Assert.That(price.Price, Is.EqualTo(expectedPrice.Price));
                Assert.That(price.ErrorMessage, Is.EqualTo("Product name cannot be empty."));
            });
        }

        [Test]
        public async Task GetPriceAsync_HttpRequestFails_LogsErrorAndReturnsZero()
        {
            // Arrange
            HttpClient httpClient = GetMockFailedHttpClient();
            var service = new PriceService(httpClient, _loggerMock.Object);

            // Act
            var result = await service.GetPriceAsync("apple");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Price, Is.EqualTo(0));
                Assert.That(result.Success, Is.False);
                Assert.That(result.ErrorMessage, Is.EqualTo("Failed to fetch price."));
            });

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to fetch price")),
                    It.IsAny<HttpRequestException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        private static HttpClient GetMockFailedHttpClient()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network failure"));

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://testendpoint.ts")
            };
        }

        private static HttpClient GetMockHttpClient(HttpResponseMessage responseMessage)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://testendpoint.ts")
            };
        }
    }
}