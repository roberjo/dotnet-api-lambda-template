using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Command to delete a product
/// </summary>
public class DeleteProductCommand : IRequest<bool>
{
    /// <summary>
    /// Product ID to delete
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User or system performing the deletion
    /// </summary>
    public string DeletedBy { get; set; } = string.Empty;
}
