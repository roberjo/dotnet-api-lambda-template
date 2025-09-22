using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Queries.GetOrders;

/// <summary>
/// Query to get a paginated list of orders
/// </summary>
public class GetOrdersQuery : IRequest<PaginatedResult<OrderDto>>
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
    /// Search term to filter by order number, customer name, or customer email
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by customer ID
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Filter by order status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by order source
    /// </summary>
    public string? OrderSource { get; set; }

    /// <summary>
    /// Filter by date range - start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter by date range - end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Sort by field (ordernumber, customerid, status, createdat, totalamount, etc.)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string? SortDirection { get; set; }
}
