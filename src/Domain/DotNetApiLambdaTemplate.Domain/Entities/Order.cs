using DotNetApiLambdaTemplate.Domain.Common;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Entities;

/// <summary>
/// Order entity representing a customer order
/// </summary>
public class Order : BaseEntity<Guid>
{
    private readonly List<OrderItem> _orderItems = new();

    /// <summary>
    /// Order number (human-readable identifier)
    /// </summary>
    public string OrderNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Customer user ID
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Customer email at the time of order
    /// </summary>
    public string CustomerEmail { get; private set; } = string.Empty;

    /// <summary>
    /// Customer name at the time of order
    /// </summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>
    /// Order status
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Order items (read-only collection)
    /// </summary>
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// Subtotal (sum of all order items)
    /// </summary>
    public Money Subtotal { get; private set; } = null!;

    /// <summary>
    /// Tax amount
    /// </summary>
    public Money TaxAmount { get; private set; } = null!;

    /// <summary>
    /// Shipping cost
    /// </summary>
    public Money ShippingCost { get; private set; } = null!;

    /// <summary>
    /// Discount amount
    /// </summary>
    public Money DiscountAmount { get; private set; } = null!;

    /// <summary>
    /// Total amount (Subtotal + Tax + Shipping - Discount)
    /// </summary>
    public Money TotalAmount { get; private set; } = null!;

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; private set; } = "USD";

    /// <summary>
    /// Shipping address
    /// </summary>
    public string? ShippingAddress { get; private set; }

    /// <summary>
    /// Billing address
    /// </summary>
    public string? BillingAddress { get; private set; }

    /// <summary>
    /// Payment method used
    /// </summary>
    public string? PaymentMethod { get; private set; }

    /// <summary>
    /// Payment transaction ID
    /// </summary>
    public string? PaymentTransactionId { get; private set; }

    /// <summary>
    /// Order notes
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Expected delivery date
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; private set; }

    /// <summary>
    /// Actual delivery date
    /// </summary>
    public DateTime? DeliveredDate { get; private set; }

    /// <summary>
    /// Shipping tracking number
    /// </summary>
    public string? TrackingNumber { get; private set; }

    /// <summary>
    /// Shipping carrier
    /// </summary>
    public string? ShippingCarrier { get; private set; }

    /// <summary>
    /// Order source (e.g., "Web", "Mobile", "API")
    /// </summary>
    public string OrderSource { get; private set; } = "Web";

    /// <summary>
    /// Promo code used
    /// </summary>
    public string? PromoCode { get; private set; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Order() { }

    /// <summary>
    /// Creates a new Order instance
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="orderNumber">Order number</param>
    /// <param name="customerId">Customer user ID</param>
    /// <param name="customerEmail">Customer email</param>
    /// <param name="customerName">Customer name</param>
    /// <param name="createdBy">User or system that created this order</param>
    /// <param name="currency">Currency code</param>
    /// <param name="shippingAddress">Shipping address</param>
    /// <param name="billingAddress">Billing address</param>
    /// <param name="orderSource">Order source</param>
    /// <param name="notes">Order notes</param>
    /// <param name="promoCode">Promo code</param>
    public Order(
        Guid id,
        string orderNumber,
        Guid customerId,
        string customerEmail,
        string customerName,
        string createdBy,
        string currency = "USD",
        string? shippingAddress = null,
        string? billingAddress = null,
        string orderSource = "Web",
        string? notes = null,
        string? promoCode = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("OrderNumber cannot be null or empty", nameof(orderNumber));
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(customerId));
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("CustomerEmail cannot be null or empty", nameof(customerEmail));
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("CustomerName cannot be null or empty", nameof(customerName));
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be null or empty", nameof(createdBy));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        OrderNumber = orderNumber.Trim();
        CustomerId = customerId;
        CustomerEmail = customerEmail.Trim();
        CustomerName = customerName.Trim();
        Currency = currency.ToUpperInvariant();
        ShippingAddress = shippingAddress?.Trim();
        BillingAddress = billingAddress?.Trim();
        OrderSource = orderSource?.Trim() ?? "Web";
        Notes = notes?.Trim();
        PromoCode = promoCode?.Trim();

        Status = OrderStatus.Pending;
        Subtotal = Money.Create(0, Currency);
        TaxAmount = Money.Create(0, Currency);
        ShippingCost = Money.Create(0, Currency);
        DiscountAmount = Money.Create(0, Currency);
        TotalAmount = Money.Create(0, Currency);

        SetCreated(createdBy);
    }

    /// <summary>
    /// Adds an item to the order
    /// </summary>
    /// <param name="orderItem">Order item to add</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void AddOrderItem(OrderItem orderItem, string updatedBy)
    {
        if (orderItem == null)
            throw new ArgumentNullException(nameof(orderItem));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to an order that is not pending");

        // Check if item already exists and update quantity
        var existingItem = _orderItems.FirstOrDefault(item => item.IsSameProduct(orderItem));
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + orderItem.Quantity;
            var updatedItem = existingItem.UpdateQuantity(newQuantity);
            _orderItems.Remove(existingItem);
            _orderItems.Add(updatedItem);
        }
        else
        {
            _orderItems.Add(orderItem);
        }

        RecalculateTotals();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Removes an item from the order
    /// </summary>
    /// <param name="productId">Product ID to remove</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void RemoveOrderItem(Guid productId, string updatedBy)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from an order that is not pending");

        var itemToRemove = _orderItems.FirstOrDefault(item => item.ProductId == productId);
        if (itemToRemove != null)
        {
            _orderItems.Remove(itemToRemove);
            RecalculateTotals();
            SetUpdated(updatedBy);
        }
    }

    /// <summary>
    /// Updates the quantity of an order item
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="newQuantity">New quantity</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateOrderItemQuantity(Guid productId, int newQuantity, string updatedBy)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot update items in an order that is not pending");

        var itemToUpdate = _orderItems.FirstOrDefault(item => item.ProductId == productId);
        if (itemToUpdate != null)
        {
            var updatedItem = itemToUpdate.UpdateQuantity(newQuantity);
            _orderItems.Remove(itemToUpdate);
            _orderItems.Add(updatedItem);
            RecalculateTotals();
            SetUpdated(updatedBy);
        }
    }

    /// <summary>
    /// Updates the order status
    /// </summary>
    /// <param name="newStatus">New status</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateStatus(OrderStatus newStatus, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        var oldStatus = Status;
        Status = newStatus;

        // Set delivery date when status changes to delivered
        if (newStatus == OrderStatus.Delivered && !DeliveredDate.HasValue)
        {
            DeliveredDate = DateTime.UtcNow;
        }

        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the shipping information
    /// </summary>
    /// <param name="shippingAddress">New shipping address</param>
    /// <param name="billingAddress">New billing address</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateShippingInfo(string? shippingAddress, string? billingAddress, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot update shipping info for an order that is not pending");

        ShippingAddress = shippingAddress?.Trim();
        BillingAddress = billingAddress?.Trim();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the payment information
    /// </summary>
    /// <param name="paymentMethod">Payment method</param>
    /// <param name="paymentTransactionId">Payment transaction ID</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdatePaymentInfo(string? paymentMethod, string? paymentTransactionId, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        PaymentMethod = paymentMethod?.Trim();
        PaymentTransactionId = paymentTransactionId?.Trim();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the shipping tracking information
    /// </summary>
    /// <param name="trackingNumber">Tracking number</param>
    /// <param name="shippingCarrier">Shipping carrier</param>
    /// <param name="expectedDeliveryDate">Expected delivery date</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateTrackingInfo(string? trackingNumber, string? shippingCarrier, DateTime? expectedDeliveryDate, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        TrackingNumber = trackingNumber?.Trim();
        ShippingCarrier = shippingCarrier?.Trim();
        ExpectedDeliveryDate = expectedDeliveryDate;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the order notes
    /// </summary>
    /// <param name="notes">New notes</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateNotes(string? notes, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Notes = notes?.Trim();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Applies a discount to the order
    /// </summary>
    /// <param name="discountAmount">Discount amount</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void ApplyDiscount(Money discountAmount, string updatedBy)
    {
        if (discountAmount == null)
            throw new ArgumentNullException(nameof(discountAmount));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));
        if (discountAmount.Currency != Currency)
            throw new ArgumentException("Discount currency must match order currency", nameof(discountAmount));
        if (discountAmount.Amount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(discountAmount));

        DiscountAmount = discountAmount;
        RecalculateTotals();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Sets the shipping cost
    /// </summary>
    /// <param name="shippingCost">Shipping cost</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void SetShippingCost(Money shippingCost, string updatedBy)
    {
        if (shippingCost == null)
            throw new ArgumentNullException(nameof(shippingCost));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));
        if (shippingCost.Currency != Currency)
            throw new ArgumentException("Shipping cost currency must match order currency", nameof(shippingCost));
        if (shippingCost.Amount < 0)
            throw new ArgumentException("Shipping cost cannot be negative", nameof(shippingCost));

        ShippingCost = shippingCost;
        RecalculateTotals();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Sets the tax amount
    /// </summary>
    /// <param name="taxAmount">Tax amount</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void SetTaxAmount(Money taxAmount, string updatedBy)
    {
        if (taxAmount == null)
            throw new ArgumentNullException(nameof(taxAmount));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));
        if (taxAmount.Currency != Currency)
            throw new ArgumentException("Tax currency must match order currency", nameof(taxAmount));
        if (taxAmount.Amount < 0)
            throw new ArgumentException("Tax amount cannot be negative", nameof(taxAmount));

        TaxAmount = taxAmount;
        RecalculateTotals();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Recalculates all order totals
    /// </summary>
    private void RecalculateTotals()
    {
        // Calculate subtotal
        var subtotalAmount = _orderItems.Sum(item => item.TotalPrice.Amount);
        Subtotal = Money.Create(subtotalAmount, Currency);

        // Calculate total amount
        var totalAmount = Subtotal.Amount + TaxAmount.Amount + ShippingCost.Amount - DiscountAmount.Amount;
        TotalAmount = Money.Create(Math.Max(0, totalAmount), Currency);
    }

    /// <summary>
    /// Gets the total number of items in the order
    /// </summary>
    /// <returns>Total quantity of all items</returns>
    public int GetTotalItemCount()
    {
        return _orderItems.Sum(item => item.Quantity);
    }

    /// <summary>
    /// Gets the total weight of the order
    /// </summary>
    /// <returns>Total weight in grams, or null if any item doesn't have weight</returns>
    public decimal? GetTotalWeight()
    {
        var totalWeight = 0m;
        foreach (var item in _orderItems)
        {
            var itemWeight = item.GetTotalWeight();
            if (!itemWeight.HasValue)
                return null;
            totalWeight += itemWeight.Value;
        }
        return totalWeight;
    }

    /// <summary>
    /// Checks if the order can be cancelled
    /// </summary>
    /// <returns>True if order can be cancelled</returns>
    public bool CanBeCancelled()
    {
        return Status == OrderStatus.Pending || Status == OrderStatus.Processing || Status == OrderStatus.Confirmed;
    }

    /// <summary>
    /// Checks if the order can be modified
    /// </summary>
    /// <returns>True if order can be modified</returns>
    public bool CanBeModified()
    {
        return Status == OrderStatus.Pending;
    }

    /// <summary>
    /// Checks if the order can be shipped
    /// </summary>
    /// <returns>True if order can be shipped</returns>
    public bool CanBeShipped()
    {
        return Status == OrderStatus.Confirmed;
    }

    /// <summary>
    /// Checks if the order can be delivered
    /// </summary>
    /// <returns>True if order can be delivered</returns>
    public bool CanBeDelivered()
    {
        return Status == OrderStatus.Shipped;
    }

    /// <summary>
    /// Checks if the order is completed
    /// </summary>
    /// <returns>True if order is completed</returns>
    public bool IsCompleted()
    {
        return Status == OrderStatus.Delivered;
    }

    /// <summary>
    /// Checks if the order is cancelled
    /// </summary>
    /// <returns>True if order is cancelled</returns>
    public bool IsCancelled()
    {
        return Status == OrderStatus.Cancelled || Status == OrderStatus.Refunded;
    }
}
