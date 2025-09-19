using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.ValueObjects;

/// <summary>
/// Order item value object representing a product in an order
/// </summary>
public sealed class OrderItem : IEquatable<OrderItem>
{
    /// <summary>
    /// Product ID
    /// </summary>
    public Guid ProductId { get; }

    /// <summary>
    /// Product name at the time of order
    /// </summary>
    public string ProductName { get; }

    /// <summary>
    /// Product SKU at the time of order
    /// </summary>
    public string ProductSku { get; }

    /// <summary>
    /// Quantity ordered
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Unit price at the time of order
    /// </summary>
    public Money UnitPrice { get; }

    /// <summary>
    /// Total price for this item (UnitPrice * Quantity)
    /// </summary>
    public Money TotalPrice { get; }

    /// <summary>
    /// Product weight in grams (for shipping calculations)
    /// </summary>
    public decimal? Weight { get; }

    /// <summary>
    /// Product dimensions (for shipping calculations)
    /// </summary>
    public string? Dimensions { get; }

    /// <summary>
    /// Product brand at the time of order
    /// </summary>
    public string? Brand { get; }

    /// <summary>
    /// Product model at the time of order
    /// </summary>
    public string? Model { get; }

    /// <summary>
    /// Product color at the time of order
    /// </summary>
    public string? Color { get; }

    /// <summary>
    /// Product size at the time of order
    /// </summary>
    public string? Size { get; }

    private OrderItem(
        Guid productId,
        string productName,
        string productSku,
        int quantity,
        Money unitPrice,
        decimal? weight = null,
        string? dimensions = null,
        string? brand = null,
        string? model = null,
        string? color = null,
        string? size = null)
    {
        ProductId = productId;
        ProductName = productName;
        ProductSku = productSku;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Weight = weight;
        Dimensions = dimensions;
        Brand = brand;
        Model = model;
        Color = color;
        Size = size;
        TotalPrice = unitPrice.Multiply(quantity);
    }

    /// <summary>
    /// Creates a new OrderItem instance with validation
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="productName">Product name</param>
    /// <param name="productSku">Product SKU</param>
    /// <param name="quantity">Quantity ordered</param>
    /// <param name="unitPrice">Unit price</param>
    /// <param name="weight">Product weight in grams</param>
    /// <param name="dimensions">Product dimensions</param>
    /// <param name="brand">Product brand</param>
    /// <param name="model">Product model</param>
    /// <param name="color">Product color</param>
    /// <param name="size">Product size</param>
    /// <returns>A new OrderItem instance</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public static OrderItem Create(
        Guid productId,
        string productName,
        string productSku,
        int quantity,
        Money unitPrice,
        decimal? weight = null,
        string? dimensions = null,
        string? brand = null,
        string? model = null,
        string? color = null,
        string? size = null)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("ProductName cannot be null or empty", nameof(productName));

        if (string.IsNullOrWhiteSpace(productSku))
            throw new ArgumentException("ProductSku cannot be null or empty", nameof(productSku));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice == null)
            throw new ArgumentNullException(nameof(unitPrice));

        if (weight.HasValue && weight.Value < 0)
            throw new ArgumentException("Weight cannot be negative", nameof(weight));

        return new OrderItem(
            productId,
            productName.Trim(),
            productSku.Trim().ToUpperInvariant(),
            quantity,
            unitPrice,
            weight,
            dimensions?.Trim(),
            brand?.Trim(),
            model?.Trim(),
            color?.Trim(),
            size?.Trim());
    }

    /// <summary>
    /// Creates an OrderItem from a Product entity
    /// </summary>
    /// <param name="product">Product entity</param>
    /// <param name="quantity">Quantity to order</param>
    /// <returns>A new OrderItem instance</returns>
    public static OrderItem FromProduct(Entities.Product product, int quantity)
    {
        return Create(
            product.Id,
            product.Name,
            product.Sku,
            quantity,
            product.Price,
            product.Weight,
            product.Dimensions,
            product.Brand,
            product.Model,
            product.Color,
            product.Size);
    }

    /// <summary>
    /// Updates the quantity of the order item
    /// </summary>
    /// <param name="newQuantity">New quantity</param>
    /// <returns>A new OrderItem with updated quantity</returns>
    public OrderItem UpdateQuantity(int newQuantity)
    {
        return Create(
            ProductId,
            ProductName,
            ProductSku,
            newQuantity,
            UnitPrice,
            Weight,
            Dimensions,
            Brand,
            Model,
            Color,
            Size);
    }

    /// <summary>
    /// Calculates the total weight for this item
    /// </summary>
    /// <returns>Total weight in grams, or null if weight is not specified</returns>
    public decimal? GetTotalWeight()
    {
        return Weight.HasValue ? Weight.Value * Quantity : null;
    }

    /// <summary>
    /// Checks if this order item has the same product as another
    /// </summary>
    /// <param name="other">Other order item</param>
    /// <returns>True if same product</returns>
    public bool IsSameProduct(OrderItem other)
    {
        return ProductId == other.ProductId;
    }

    /// <summary>
    /// Equality comparison
    /// </summary>
    public bool Equals(OrderItem? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ProductId == other.ProductId &&
               ProductName == other.ProductName &&
               ProductSku == other.ProductSku &&
               Quantity == other.Quantity &&
               UnitPrice.Equals(other.UnitPrice) &&
               Weight == other.Weight &&
               Dimensions == other.Dimensions &&
               Brand == other.Brand &&
               Model == other.Model &&
               Color == other.Color &&
               Size == other.Size;
    }

    /// <summary>
    /// Equality comparison
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as OrderItem);
    }

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(ProductId);
        hash.Add(ProductName);
        hash.Add(ProductSku);
        hash.Add(Quantity);
        hash.Add(UnitPrice);
        hash.Add(Weight);
        hash.Add(Dimensions);
        hash.Add(Brand);
        hash.Add(Model);
        hash.Add(Color);
        hash.Add(Size);
        return hash.ToHashCode();
    }

    /// <summary>
    /// String representation
    /// </summary>
    public override string ToString()
    {
        return $"{ProductName} (SKU: {ProductSku}) x{Quantity} @ {UnitPrice} = {TotalPrice}";
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(OrderItem? left, OrderItem? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(OrderItem? left, OrderItem? right)
    {
        return !Equals(left, right);
    }
}
