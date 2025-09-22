using MediatR;
using Microsoft.AspNetCore.Mvc;
using DotNetApiLambdaTemplate.Application.Users.Commands.CreateUser;
using DotNetApiLambdaTemplate.Application.Users.Commands.UpdateUser;
using DotNetApiLambdaTemplate.Application.Users.Commands.DeleteUser;
using DotNetApiLambdaTemplate.Application.Users.Queries.GetUsers;
using DotNetApiLambdaTemplate.Application.Users.Queries.GetUserById;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.API.Controllers;

/// <summary>
/// Controller for managing users
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all users with optional filtering and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>List of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<UserDto>>> GetUsers([FromQuery] GetUsersQuery query)
    {
        _logger.LogInformation("Getting users with query: {Query}", query);

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved {Count} users", result.Value.Items.Count);
            return Ok(result.Value);
        }

        _logger.LogWarning("Failed to retrieve users: {Errors}", string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);

        var query = new GetUserByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved user: {UserId}", id);
            return Ok(result.Value);
        }

        _logger.LogWarning("User not found: {UserId}", id);
        return NotFound();
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="command">User creation command</param>
    /// <returns>Created user details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserCommand command)
    {
        _logger.LogInformation("Creating user with email: {Email}", command.Email);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully created user: {UserId}", result.Value.Id);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Value.Id }, result.Value);
        }

        _logger.LogWarning("Failed to create user: {Errors}", string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">User update command</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        _logger.LogInformation("Updating user: {UserId}", id);

        command.Id = id; // Ensure the ID from the route is used
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully updated user: {UserId}", id);
            return Ok(result.Value);
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("User not found for update: {UserId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to update user {UserId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        _logger.LogInformation("Deleting user: {UserId}", id);

        var command = new DeleteUserCommand { Id = id };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully deleted user: {UserId}", id);
            return NoContent();
        }

        if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("User not found for deletion: {UserId}", id);
            return NotFound();
        }

        _logger.LogWarning("Failed to delete user {UserId}: {Errors}", id, string.Join(", ", result.Errors));
        return BadRequest(result.Errors);
    }
}
