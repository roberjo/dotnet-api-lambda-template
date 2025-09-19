using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.Services;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using Moq;
using Xunit;

namespace DotNetApiLambdaTemplate.Domain.Tests.Services;

public class OrderDomainServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly OrderDomainService _orderDomainService;

    public OrderDomainServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _orderDomainService = new OrderDomainService(
            _mockOrderRepository.Object,
            _mockProductRepository.Object,
            _mockUserRepository.Object);
    }

    [Fact]
    public async Task CanCreateOrderAsync_WhenCustomerIsActive_ReturnsTrue()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = CreateTestUser(id: customerId, isActive: true);
        _mockUserRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockOrderRepository.Setup(x => x.GetPendingOrdersByCustomerAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _orderDomainService.CanCreateOrderAsync(customerId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanCreateOrderAsync_WhenCustomerIsInactive_ReturnsFalse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = CreateTestUser(id: customerId, isActive: false);
        _mockUserRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _orderDomainService.CanCreateOrderAsync(customerId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanCreateOrderAsync_WhenCustomerHasTooManyPendingOrders_ReturnsFalse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = CreateTestUser(id: customerId, isActive: true);
        var pendingOrders = Enumerable.Range(0, 6).Select(_ => CreateTestOrder()).ToList();

        _mockUserRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockOrderRepository.Setup(x => x.GetPendingOrdersByCustomerAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingOrders);

        // Act
        var result = await _orderDomainService.CanCreateOrderAsync(customerId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanUpdateOrderAsync_WhenOrderCanBeModified_ReturnsTrue()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Pending);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanUpdateOrderAsync(orderId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanUpdateOrderAsync_WhenOrderCannotBeModified_ReturnsFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Shipped);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanUpdateOrderAsync(orderId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanCancelOrderAsync_WhenOrderCanBeCancelled_ReturnsTrue()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Pending);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanCancelOrderAsync(orderId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanCancelOrderAsync_WhenOrderCannotBeCancelled_ReturnsFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Delivered);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanCancelOrderAsync(orderId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanShipOrderAsync_WhenOrderIsConfirmed_ReturnsTrue()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Confirmed);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanShipOrderAsync(orderId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanShipOrderAsync_WhenOrderIsNotConfirmed_ReturnsFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Pending);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanShipOrderAsync(orderId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanDeliverOrderAsync_WhenOrderIsShipped_ReturnsTrue()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Shipped);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanDeliverOrderAsync(orderId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanDeliverOrderAsync_WhenOrderIsNotShipped_ReturnsFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(id: orderId, status: OrderStatus.Confirmed);
        _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderDomainService.CanDeliverOrderAsync(orderId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CalculateShippingCostAsync_WithEmptyItems_ReturnsZeroCost()
    {
        // Arrange
        var orderItems = new List<OrderItem>();
        var shippingAddress = "123 Main St, City, State 12345";

        // Act
        var result = await _orderDomainService.CalculateShippingCostAsync(orderItems, shippingAddress);

        // Assert
        Assert.Equal(0, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public async Task CalculateShippingCostAsync_WithItems_ReturnsCalculatedCost()
    {
        // Arrange
        var orderItems = new List<OrderItem>
        {
            CreateTestOrderItem(weight: 0.5m),
            CreateTestOrderItem(weight: 0.3m)
        };
        var shippingAddress = "123 Main St, City, State 12345";

        // Act
        var result = await _orderDomainService.CalculateShippingCostAsync(orderItems, shippingAddress);

        // Assert
        Assert.True(result.Amount > 0);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public async Task CalculateTaxAmountAsync_WithValidInput_ReturnsCalculatedTax()
    {
        // Arrange
        var subtotal = Money.Create(100.00m, "USD");
        var shippingAddress = "123 Main St, Los Angeles, CA 90210";

        // Act
        var result = await _orderDomainService.CalculateTaxAmountAsync(subtotal, shippingAddress);

        // Assert
        Assert.True(result.Amount > 0);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public async Task GenerateOrderNumberAsync_ReturnsUniqueOrderNumber()
    {
        // Arrange
        _mockOrderRepository.Setup(x => x.GetByOrderNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _orderDomainService.GenerateOrderNumberAsync();

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("ORD-", result);
    }

    [Fact]
    public async Task GetValidNextStatusesAsync_ForPendingOrder_ReturnsCorrectStatuses()
    {
        // Act
        var result = await _orderDomainService.GetValidNextStatusesAsync(OrderStatus.Pending);

        // Assert
        Assert.Contains(OrderStatus.Processing, result);
        Assert.Contains(OrderStatus.Cancelled, result);
        Assert.DoesNotContain(OrderStatus.Delivered, result);
    }

    [Fact]
    public async Task GetValidNextStatusesAsync_ForShippedOrder_ReturnsCorrectStatuses()
    {
        // Act
        var result = await _orderDomainService.GetValidNextStatusesAsync(OrderStatus.Shipped);

        // Assert
        Assert.Contains(OrderStatus.Delivered, result);
        Assert.Contains(OrderStatus.Returned, result);
        Assert.DoesNotContain(OrderStatus.Pending, result);
    }

    private static User CreateTestUser(
        Guid? id = null,
        string email = "test@example.com",
        string firstName = "Test",
        string lastName = "User",
        UserRole role = UserRole.User,
        bool isActive = true)
    {
        var userId = id ?? Guid.NewGuid();
        var userEmail = Email.Create(email);
        var userName = FullName.Create(firstName, lastName);

        return new User(userId, userName, userEmail, role, "System");
    }

    private static Order CreateTestOrder(
        Guid? id = null,
        Guid? customerId = null,
        OrderStatus status = OrderStatus.Pending)
    {
        var orderId = id ?? Guid.NewGuid();
        var orderCustomerId = customerId ?? Guid.NewGuid();

        return new Order(
            orderId,
            "ORD-001",
            orderCustomerId,
            "test@example.com",
            "Test Customer",
            "System");
    }

    private static OrderItem CreateTestOrderItem(
        Guid? productId = null,
        string productName = "Test Product",
        string productSku = "TEST-001",
        int quantity = 1,
        decimal? unitPrice = 10.00m,
        decimal? weight = null)
    {
        var id = productId ?? Guid.NewGuid();
        var price = Money.Create(unitPrice ?? 10.00m, "USD");

        return OrderItem.Create(
            id,
            productName,
            productSku,
            quantity,
            price,
            weight);
    }
}