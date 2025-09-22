using DotNetApiLambdaTemplate.Domain.Enums;

namespace DotNetApiLambdaTemplate.Application.Common.DTOs;

/// <summary>
/// Data Transfer Object for Order entity
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Order unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Order number (human-readable identifier)
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer user ID
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Customer email at the time of order
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Customer name at the time of order
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Order status
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Order items
    /// </summary>
    public List<OrderItemDto> OrderItems { get; set; } = new();

    /// <summary>
    /// Subtotal amount
    /// </summary>
    public decimal SubtotalAmount { get; set; }

    /// <summary>
    /// Subtotal currency
    /// </summary>
    public string SubtotalCurrency { get; set; } = "USD";

    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Tax currency
    /// </summary>
    public string TaxCurrency { get; set; } = "USD";

    /// <summary>
    /// Shipping cost amount
    /// </summary>
    public decimal ShippingCostAmount { get; set; }

    /// <summary>
    /// Shipping cost currency
    /// </summary>
    public string ShippingCostCurrency { get; set; } = "USD";

    /// <summary>
    /// Discount amount applied to the order
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Discount currency
    /// </summary>
    public string DiscountCurrency { get; set; } = "USD";

    /// <summary>
    /// Total amount of the order (Subtotal + Tax + Shipping - Discount)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Total currency
    /// </summary>
    public string TotalCurrency { get; set; } = "USD";

    /// <summary>
    /// Shipping address
    /// </summary>
    public string? ShippingAddress { get; set; }

    /// <summary>
    /// Billing address
    /// </summary>
    public string? BillingAddress { get; set; }

    /// <summary>
    /// Payment method used
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Payment transaction ID
    /// </summary>
    public string? PaymentTransactionId { get; set; }

    /// <summary>
    /// Order notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Expected delivery date
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// Actual delivery date
    /// </summary>
    public DateTime? DeliveredDate { get; set; }

    /// <summary>
    /// Shipping tracking number
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// Shipping carrier
    /// </summary>
    public string? ShippingCarrier { get; set; }

    /// <summary>
    /// Order source (e.g., "Web", "Mobile", "API")
    /// </summary>
    public string OrderSource { get; set; } = "Web";

    /// <summary>
    /// Promo code used
    /// </summary>
    public string? PromoCode { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// User who created this record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// User who last updated this record
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for OrderItem value object
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Product name at the time of order
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Product SKU at the time of order
    /// </summary>
    public string ProductSku { get; set; } = string.Empty;

    /// <summary>
    /// Quantity ordered
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price at the time of order
    /// </summary>
    public decimal UnitPriceAmount { get; set; }

    /// <summary>
    /// Unit price currency
    /// </summary>
    public string UnitPriceCurrency { get; set; } = "USD";

    /// <summary>
    /// Total price for this item (UnitPrice * Quantity)
    /// </summary>
    public decimal TotalPriceAmount { get; set; }

    /// <summary>
    /// Total price currency
    /// </summary>
    public string TotalPriceCurrency { get; set; } = "USD";

    /// <summary>
    /// Product weight at the time of order (optional)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Product dimensions at the time of order (optional)
    /// </summary>
    public string? Dimensions { get; set; }

    /// <summary>
    /// Product brand at the time of order (optional)
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Product model at the time of order (optional)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Product color at the time of order (optional)
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Product size at the time of order (optional)
    /// </summary>
    public string? Size { get; set; }
}
