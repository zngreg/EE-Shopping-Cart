using EE.ShoppingCart.Cart;
using EE.ShoppingCart.Models;
using EE.ShoppingCart.Services;
using Moq;

namespace EE.ShoppingCart.Unit.Tests
{
    [TestFixture]
    public class CartTests
    {
        private Mock<IPriceService> _priceServiceMock;

        [SetUp]
        public void SetUp()
        {
            _priceServiceMock = new Mock<IPriceService>();
        }

        [Test]
        public async Task AddItemAsync_With_ValidProducts_AddsItemToCart_And_CalculatesTotalsCorrectly()
        {
            // Arrange
            _priceServiceMock.Setup(s => s.GetPriceAsync("cornflakes")).ReturnsAsync(new PriceResult(true, 2.52M, null));
            _priceServiceMock.Setup(s => s.GetPriceAsync("weetabix")).ReturnsAsync(new PriceResult(true, 9.98M, null));

            var cart = new CartProcessor.Cart(_priceServiceMock.Object);

            // Act
            await cart.AddItemAsync("cornflakes", 1);
            await cart.AddItemAsync("cornflakes", 1);
            await cart.AddItemAsync("weetabix", 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cart.Subtotal, Is.EqualTo(15.02M));
                Assert.That(cart.Tax, Is.EqualTo(1.88M));
                Assert.That(cart.Total, Is.EqualTo(16.90M));
                Assert.That(cart.Items.Count, Is.EqualTo(2));
                Assert.That(cart.Items.First(i => i.ProductName == "cornflakes").Quantity, Is.EqualTo(2));
                Assert.That(cart.Items.First(i => i.ProductName == "weetabix").Quantity, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task AddItemAsync_With_ValidProducts_With_CustomTaxRate_AddsItemToCart_And_CalculatesTotalsCorrectly()
        {
            // Arrange
            _priceServiceMock.Setup(s => s.GetPriceAsync("cornflakes")).ReturnsAsync(new PriceResult(true, 2.52M, null));

            var cart = new CartProcessor.Cart(_priceServiceMock.Object);

            // Act
            cart.TaxRate = 0.25M;
            await cart.AddItemAsync("cornflakes", 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cart.Subtotal, Is.EqualTo(2.52M));
                Assert.That(cart.Tax, Is.EqualTo(0.63M));
                Assert.That(cart.Total, Is.EqualTo(3.15M));
                Assert.That(cart.Items.Count, Is.EqualTo(1));
                Assert.That(cart.Items.First(i => i.ProductName == "cornflakes").Quantity, Is.EqualTo(1));
            });
        }

        [Test]
        public void AddItemAsync_With_InvalidProduct_ThrowsArgumentException()
        {
            // Arrange
            _priceServiceMock.Setup(s => s.GetPriceAsync("invalidProduct")).ReturnsAsync(new PriceResult(false, 0, "Product not found."));

            var cart = new CartProcessor.Cart(_priceServiceMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await cart.AddItemAsync("invalidProduct", 0), "Product 'invalidProduct' is not available.");
        }

        [Test]
        public void AddItemAsync_With_NonExistentProduct_ThrowsArgumentException()
        {
            // Arrange
            _priceServiceMock.Setup(s => s.GetPriceAsync("nonExistentProduct")).ReturnsAsync(new PriceResult(false, 0, "Product not found."));

            var cart = new CartProcessor.Cart(_priceServiceMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await cart.AddItemAsync("nonExistentProduct", 1), "Product 'nonExistentProduct' is not available.");
        }

        [Test]
        public void AddItemAsync_With_EmptyProduct_ThrowsArgumentException()
        {
            // Arrange
            var cart = new CartProcessor.Cart(_priceServiceMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await cart.AddItemAsync("", 1), "Product name cannot be empty and quantity must be greater than zero.");
        }

        [Test]
        public void AddItemAsync_With_NegativeQuantity_ThrowsArgumentException()
        {
            // Arrange
            var cart = new CartProcessor.Cart(_priceServiceMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await cart.AddItemAsync("cornflakes", -1), "Product name cannot be empty and quantity must be greater than zero.");
        }

        [Test]
        public async Task RemoveItemsAsync_With_ValidProdctName_And_Quantity_Should_RemoveItemFromCartAsync()
        {
            // Arrange
            _priceServiceMock.Setup(s => s.GetPriceAsync("cornflakes")).ReturnsAsync(new PriceResult(true, 2.52M, null));

            var service = new CartProcessor.Cart(_priceServiceMock.Object);
            await service.AddItemAsync("cornflakes", 1);
            await service.AddItemAsync("cornflakes", 1);

            // Act
            var results = await service.RemoveItemAsync("cornflakes", 1);

            // Assert
            Assert.That(results, Is.True);
        }

        [Test]
        public async Task RemoveItemsAsync_With_ValidProductName_And_Zero_Quanitity_It_Should_Throw_An_ArgumentException()
        {
            // Arrange
            _priceServiceMock.Setup(s => s.GetPriceAsync("cornflakes")).ReturnsAsync(new PriceResult(true, 2.52M, null));

            var _service = new CartProcessor.Cart(_priceServiceMock.Object);
            await _service.AddItemAsync("cornflakes", 1);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.RemoveItemAsync("cornflakes", 0), "Product name cannot be empty and quantity must be greater than zero.");
        }

        public async Task RemoveItemsAsync_With_NotFoundProductName_It_Should_Not_Update_Quantity()
        {
            // Arrange
            _priceServiceMock.Setup(s => s.GetPriceAsync("cornflakes")).ReturnsAsync(new PriceResult(true, 2.52M, null));

            var service = new CartProcessor.Cart(_priceServiceMock.Object);
            await service.AddItemAsync("cornflakes", 1);
            await service.AddItemAsync("cornflakes", 1);

            // Act
            var results = await service.RemoveItemAsync("bigcornbites", 1);

            // Assert
            Assert.That(results, Is.False);
            Assert.That(service.Items.Count, Is.EqualTo(2));
        }
    }
}