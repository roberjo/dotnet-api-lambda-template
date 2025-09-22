using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Products.Commands.AdjustProductInventory;

/// <summary>
/// Command to adjust product inventory
/// </summary>
public class AdjustProductInventoryCommand : IRequest<ProductDto>
{
    /// <summary>
    /// Product ID to adjust inventory for
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Quantity change (positive for increase, negative for decrease)
    /// </summary>
    public int QuantityChange { get; set; }
}
