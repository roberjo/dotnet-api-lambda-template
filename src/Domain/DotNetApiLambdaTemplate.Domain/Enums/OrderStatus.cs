namespace DotNetApiLambdaTemplate.Domain.Enums;

/// <summary>
/// Order status enumeration
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order has been created but not yet processed
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Order is being processed and items are being prepared
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Order has been confirmed and payment processed
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Order items are being prepared for shipment
    /// </summary>
    Preparing = 3,

    /// <summary>
    /// Order has been shipped
    /// </summary>
    Shipped = 4,

    /// <summary>
    /// Order has been delivered
    /// </summary>
    Delivered = 5,

    /// <summary>
    /// Order has been cancelled
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Order has been refunded
    /// </summary>
    Refunded = 7,

    /// <summary>
    /// Order is on hold (e.g., payment issues, inventory problems)
    /// </summary>
    OnHold = 8,

    /// <summary>
    /// Order has been returned
    /// </summary>
    Returned = 9
}
