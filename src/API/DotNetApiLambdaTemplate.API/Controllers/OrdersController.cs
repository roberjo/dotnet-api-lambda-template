using MediatR;
using Microsoft.AspNetCore.Mvc;
using DotNetApiLambdaTemplate.Application.Orders.Commands.CreateOrder;
using DotNetApiLambdaTemplate.Application.Orders.Commands.UpdateOrder;
using DotNetApiLambdaTemplate.Application.Orders.Commands.DeleteOrder;
using DotNetApiLambdaTemplate.Application.Orders.Commands.AddOrderItem;
using DotNetApiLambdaTemplate.Application.Orders.Commands.RemoveOrderItem;
using DotNetApiLambdaTemplate.Application.Orders.Commands.UpdateOrderItemQuantity;
using DotNetApiLambdaTemplate.Application.Orders.Queries.GetOrders;
using DotNetApiLambdaTemplate.Application.Orders.Queries.GetOrderById;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.API.Controllers;

/// <summary>
/// Controller for managing orders
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all orders with optional filtering and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>List of orders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<OrderDto>>> GetOrders([FromQuery] GetOrdersQuery query)
    {
        _logger.LogInformation("Getting orders with query: {Query}", query);

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved {Count} orders", result.Value.Items.Count);
            return Ok(result.Value);
        }

        _logger.LogWarning("Failed to retrieve orders: {Errors}", string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
    {
        _logger.LogInformation("Getting order with ID: {OrderId}", id);

        var query = new GetOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved order: {OrderId}", id);
            return Ok(result.Value);
        }

        _logger.LogWarning("Order not found: {OrderId}", id);
        return NotFound();
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="command">Order creation command</param>
    /// <returns>Created order details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        _logger.LogInformation("Creating order for customer: {CustomerId}", command.CustomerId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully created order: {OrderId}", result.Value.Id);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Value.Id }, result.Value);
        }

        _logger.LogWarning("Failed to create order: {Errors}", string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="command">Order update command</param>
    /// <returns>Updated order details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> UpdateOrder(Guid id, [FromBody] UpdateOrderCommand command)
    {
        _logger.LogInformation("Updating order: {OrderId}", id);

        command.Id = id; // Ensure the ID from the route is used
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully updated order: {OrderId}", id);
            return Ok(result.Value);
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Order not found for update: {OrderId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to update order {OrderId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Deletes an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        _logger.LogInformation("Deleting order: {OrderId}", id);

        var command = new DeleteOrderCommand { Id = id };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully deleted order: {OrderId}", id);
            return NoContent();
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Order not found for deletion: {OrderId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to delete order {OrderId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Adds an item to an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="command">Add order item command</param>
    /// <returns>Updated order details</returns>
    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> AddOrderItem(Guid id, [FromBody] AddOrderItemCommand command)
    {
        _logger.LogInformation("Adding item to order: {OrderId}", id);

        command.OrderId = id; // Ensure the ID from the route is used
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully added item to order: {OrderId}", id);
            return Ok(result.Value);
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Order not found for adding item: {OrderId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to add item to order {OrderId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Removes an item from an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="productId">Product ID to remove</param>
    /// <returns>Updated order details</returns>
    [HttpDelete("{id:guid}/items/{productId:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> RemoveOrderItem(Guid id, Guid productId)
    {
        _logger.LogInformation("Removing item from order: {OrderId}, Product: {ProductId}", id, productId);

        var command = new RemoveOrderItemCommand { OrderId = id, ProductId = productId };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully removed item from order: {OrderId}", id);
            return Ok(result.Value);
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Order or item not found for removal: {OrderId}, {ProductId}", id, productId);
            return NotFound();
        }

        _logger.LogWarning("Failed to remove item from order {OrderId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Updates the quantity of an order item
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="productId">Product ID</param>
    /// <param name="command">Update order item quantity command</param>
    /// <returns>Updated order details</returns>
    [HttpPatch("{id:guid}/items/{productId:guid}/quantity")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> UpdateOrderItemQuantity(Guid id, Guid productId, [FromBody] UpdateOrderItemQuantityCommand command)
    {
        _logger.LogInformation("Updating item quantity in order: {OrderId}, Product: {ProductId}", id, productId);

        command.OrderId = id;
        command.ProductId = productId;
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully updated item quantity in order: {OrderId}", id);
            return Ok(result.Value);
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Order or item not found for quantity update: {OrderId}, {ProductId}", id, productId);
            return NotFound();
        }

        _logger.LogWarning("Failed to update item quantity in order {OrderId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }
}
