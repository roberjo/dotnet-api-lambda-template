using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Command to update an existing product
/// </summary>
public class UpdateProductCommand : IRequest<ProductDto>
{
    /// <summary>
    /// Product ID to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// New product name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// New product description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// New SKU
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// New product category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// New selling price amount
    /// </summary>
    public decimal? PriceAmount { get; set; }

    /// <summary>
    /// New price currency
    /// </summary>
    public string? PriceCurrency { get; set; }

    /// <summary>
    /// New cost amount (optional, for internal use)
    /// </summary>
    public decimal? CostAmount { get; set; }

    /// <summary>
    /// New cost currency
    /// </summary>
    public string? CostCurrency { get; set; }

    /// <summary>
    /// New product weight in kilograms (optional)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// New product dimensions (e.g., "10x5x2 cm") (optional)
    /// </summary>
    public string? Dimensions { get; set; }

    /// <summary>
    /// New product brand (optional)
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// New product model (optional)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// New product color (optional)
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// New product size (e.g., "S", "M", "L", "One Size") (optional)
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// New comma-separated tags for the product (optional)
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// New comma-separated URLs of product images (optional)
    /// </summary>
    public string? ImageUrls { get; set; }

    /// <summary>
    /// New JSON string of product specifications (optional)
    /// </summary>
    public string? Specifications { get; set; }

    /// <summary>
    /// New warranty period in months (optional)
    /// </summary>
    public int? WarrantyMonths { get; set; }
}
