using MediatR;
using Microsoft.AspNetCore.Mvc;
using DotNetApiLambdaTemplate.Application.Products.Commands.CreateProduct;
using DotNetApiLambdaTemplate.Application.Products.Commands.UpdateProduct;
using DotNetApiLambdaTemplate.Application.Products.Commands.DeleteProduct;
using DotNetApiLambdaTemplate.Application.Products.Commands.AdjustProductInventory;
using DotNetApiLambdaTemplate.Application.Products.Queries.GetProducts;
using DotNetApiLambdaTemplate.Application.Products.Queries.GetProductById;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.API.Controllers;

/// <summary>
/// Controller for managing products
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all products with optional filtering and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>List of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts([FromQuery] GetProductsQuery query)
    {
        _logger.LogInformation("Getting products with query: {Query}", query);

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved {Count} products", result.Value.Items.Count);
            return Ok(result.Value);
        }

        _logger.LogWarning("Failed to retrieve products: {Errors}", string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved product: {ProductId}", id);
            return Ok(result.Value);
        }

        _logger.LogWarning("Product not found: {ProductId}", id);
        return NotFound();
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="command">Product creation command</param>
    /// <returns>Created product details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductCommand command)
    {
        _logger.LogInformation("Creating product with name: {ProductName}", command.Name);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully created product: {ProductId}", result.Value.Id);
            return CreatedAtAction(nameof(GetProductById), new { id = result.Value.Id }, result.Value);
        }

        _logger.LogWarning("Failed to create product: {Errors}", string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="command">Product update command</param>
    /// <returns>Updated product details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        _logger.LogInformation("Updating product: {ProductId}", id);

        command.Id = id; // Ensure the ID from the route is used
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully updated product: {ProductId}", id);
            return Ok(result.Value);
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Product not found for update: {ProductId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to update product {ProductId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        _logger.LogInformation("Deleting product: {ProductId}", id);

        var command = new DeleteProductCommand { Id = id };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully deleted product: {ProductId}", id);
            return NoContent();
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Product not found for deletion: {ProductId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to delete product {ProductId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Adjusts product inventory
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="command">Inventory adjustment command</param>
    /// <returns>Updated product details</returns>
    [HttpPatch("{id:guid}/inventory")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDto>> AdjustInventory(Guid id, [FromBody] AdjustProductInventoryCommand command)
    {
        _logger.LogInformation("Adjusting inventory for product: {ProductId}", id);

        command.ProductId = id; // Ensure the ID from the route is used
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully adjusted inventory for product: {ProductId}", id);
            return Ok(result.Value);
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Product not found for inventory adjustment: {ProductId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to adjust inventory for product {ProductId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }
}
