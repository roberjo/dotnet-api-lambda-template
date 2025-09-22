using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Products.Queries.GetProducts;

/// <summary>
/// Query to get a paginated list of products
/// </summary>
public class GetProductsQuery : IRequest<PaginatedResult<ProductDto>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Search term to filter by name, description, or SKU
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filter by featured status
    /// </summary>
    public bool? IsFeatured { get; set; }

    /// <summary>
    /// Sort by field (name, price, createdat, etc.)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string? SortDirection { get; set; }
}
