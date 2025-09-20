using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.Services;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using Moq;
using Xunit;

namespace DotNetApiLambdaTemplate.Domain.Tests.Services;

public class ProductDomainServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly ProductDomainService _productDomainService;

    public ProductDomainServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _productDomainService = new ProductDomainService(
            _mockProductRepository.Object,
            _mockOrderRepository.Object);
    }

    [Fact]
    public async Task CanCreateProductAsync_WhenSkuIsAvailable_ReturnsTrue()
    {
        // Arrange
        var sku = "TEST-001";
        _mockProductRepository.Setup(x => x.GetBySkuAsync(sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productDomainService.CanCreateProductAsync(sku);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanCreateProductAsync_WhenSkuIsNotAvailable_ReturnsFalse()
    {
        // Arrange
        var sku = "TEST-001";
        var existingProduct = CreateTestProduct(sku: sku);
        _mockProductRepository.Setup(x => x.GetBySkuAsync(sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _productDomainService.CanCreateProductAsync(sku);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanUpdateProductAsync_WhenProductExists_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productDomainService.CanUpdateProductAsync(productId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanUpdateProductAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productDomainService.CanUpdateProductAsync(productId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanDeactivateProductAsync_WhenProductHasNoPendingOrders_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId, isActive: true);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockOrderRepository.Setup(x => x.GetByStatusAsync(OrderStatus.Pending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _productDomainService.CanDeactivateProductAsync(productId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanDeleteProductAsync_WhenProductHasNoOrders_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockOrderRepository.Setup(x => x.GetByStatusAsync(OrderStatus.Pending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _productDomainService.CanDeleteProductAsync(productId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanAdjustInventoryAsync_WhenAdjustmentIsValid_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId, stockQuantity: 10, isActive: true);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productDomainService.CanAdjustInventoryAsync(productId, -5);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanAdjustInventoryAsync_WhenAdjustmentWouldResultInNegativeStock_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId, stockQuantity: 5, isActive: true);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productDomainService.CanAdjustInventoryAsync(productId, -10);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanFeatureProductAsync_WhenProductMeetsCriteria_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId, stockQuantity: 10, rating: 4.5, isActive: true);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productDomainService.CanFeatureProductAsync(productId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanFeatureProductAsync_WhenProductHasNoStock_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId, stockQuantity: 0, rating: 4.5, isActive: true);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productDomainService.CanFeatureProductAsync(productId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanFeatureProductAsync_WhenProductHasLowRating_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId, stockQuantity: 10, rating: 2.5, isActive: true);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productDomainService.CanFeatureProductAsync(productId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidatePricingAsync_WithValidPrice_ReturnsValidResult()
    {
        // Arrange
        var price = Money.Create(100.00m, "USD");
        var cost = Money.Create(50.00m, "USD");

        // Act
        var result = await _productDomainService.ValidatePricingAsync(price, cost);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.ProfitMargin);
        Assert.Equal(50, result.ProfitMargin);
    }

    [Fact]
    public void ValidatePricingAsync_WithNegativePrice_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.Create(-10.00m, "USD"));
    }

    [Fact]
    public async Task ValidatePricingAsync_WithPriceLessThanCost_ReturnsInvalidResult()
    {
        // Arrange
        var price = Money.Create(50.00m, "USD");
        var cost = Money.Create(100.00m, "USD");

        // Act
        var result = await _productDomainService.ValidatePricingAsync(price, cost);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Price must be greater than cost", result.Errors);
    }

    [Fact]
    public async Task ValidatePricingAsync_WithDifferentCurrencies_ReturnsInvalidResult()
    {
        // Arrange
        var price = Money.Create(100.00m, "USD");
        var cost = Money.Create(50.00m, "EUR");

        // Act
        var result = await _productDomainService.ValidatePricingAsync(price, cost);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Cost currency must match price currency", result.Errors);
    }

    [Fact]
    public async Task GenerateUniqueSkuAsync_ReturnsUniqueSku()
    {
        // Arrange
        _mockProductRepository.Setup(x => x.GetBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productDomainService.GenerateUniqueSkuAsync(ProductCategory.Electronics, "TestBrand");

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("ELC-", result);
    }

    [Fact]
    public async Task GetLowStockProductsAsync_ReturnsProductsBelowThreshold()
    {
        // Arrange
        var products = new List<Product>
        {
            CreateTestProduct(stockQuantity: 5),
            CreateTestProduct(stockQuantity: 15),
            CreateTestProduct(stockQuantity: 3)
        };
        _mockProductRepository.Setup(x => x.GetActiveProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productDomainService.GetLowStockProductsAsync(10);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.True(p.StockQuantity <= 10));
    }

    private static Product CreateTestProduct(
        Guid? id = null,
        string name = "Test Product",
        string sku = "TEST-001",
        decimal? price = 10.00m,
        ProductCategory category = ProductCategory.Electronics,
        int stockQuantity = 10,
        double rating = 4.0,
        bool isActive = true)
    {
        var productId = id ?? Guid.NewGuid();
        var productPrice = Money.Create(price ?? 10.00m, "USD");

        var product = new Product(
            productId,
            name,
            "Test Description",
            sku,
            category,
            productPrice,
            "System",
            stockQuantity: stockQuantity);

        if (!isActive)
        {
            product.Deactivate("System");
        }

        // Set rating if it's not the default
        if (rating != 0)
        {
            product.UpdateRating((decimal)rating, 1, "System");
        }

        return product;
    }
}