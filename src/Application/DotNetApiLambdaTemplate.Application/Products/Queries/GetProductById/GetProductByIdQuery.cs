using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Products.Queries.GetProductById;

/// <summary>
/// Query to get a product by ID
/// </summary>
public class GetProductByIdQuery : IRequest<ProductDto?>
{
    /// <summary>
    /// Product ID
    /// </summary>
    public Guid Id { get; set; }
}
