using EE.ShoppingCart.Services;
using Moq;

namespace EE.ShoppingCart.Unit.Tests
{
    [TestFixture]
    public class CartTests
    {
        [Test]
        public async Task AddItemAsync_With_ValidProducts_AddsItemToCart_And_CalculatesTotalsCorrectly()
        {
            var mockPriceService = new Mock<IPriceService>();
            mockPriceService.Setup(s => s.GetPriceAsync("cornflakes")).ReturnsAsync(2.52M);
            mockPriceService.Setup(s => s.GetPriceAsync("weetabix")).ReturnsAsync(9.98M);

            var cart = new CartProcessor.Cart(mockPriceService.Object);
            await cart.AddItemAsync("cornflakes", 1);
            await cart.AddItemAsync("cornflakes", 1);
            await cart.AddItemAsync("weetabix", 1);

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
            var mockPriceService = new Mock<IPriceService>();
            mockPriceService.Setup(s => s.GetPriceAsync("cornflakes")).ReturnsAsync(2.52M);

            var cart = new CartProcessor.Cart(mockPriceService.Object);
            cart.TaxRate = 0.25M;
            await cart.AddItemAsync("cornflakes", 1);

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
        public async Task AddItemAsync_With_InvalidProduct_ThrowsArgumentException()
        {
            var mockPriceService = new Mock<IPriceService>();
            mockPriceService.Setup(s => s.GetPriceAsync("invalidProduct")).ReturnsAsync(0);

            var cart = new CartProcessor.Cart(mockPriceService.Object);

            Assert.ThrowsAsync<ArgumentException>(async () => await cart.AddItemAsync("invalidProduct", 0), "Product 'invalidProduct' is not available.");
        }

        [Test]
        public void AddItemAsync_With_EmptyProduct_ThrowsArgumentException()
        {
            var mockPriceService = new Mock<IPriceService>();
            var cart = new CartProcessor.Cart(mockPriceService.Object);

            Assert.ThrowsAsync<ArgumentException>(async () => await cart.AddItemAsync("", 1), "Product name cannot be empty and quantity must be greater than zero.");
        }

        [Test]
        public void AddItemAsync_With_NegativeQuantity_ThrowsArgumentException()
        {
            var mockPriceService = new Mock<IPriceService>();
            var cart = new CartProcessor.Cart(mockPriceService.Object);

            Assert.ThrowsAsync<ArgumentException>(async () => await cart.AddItemAsync("cornflakes", -1), "Product name cannot be empty and quantity must be greater than zero.");
        }
    }
}