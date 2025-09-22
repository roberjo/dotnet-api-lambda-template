using MediatR;
using DotNetApiLambdaTemplate.Application.Common.Interfaces;

namespace DotNetApiLambdaTemplate.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Handler for DeleteProductCommand
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
