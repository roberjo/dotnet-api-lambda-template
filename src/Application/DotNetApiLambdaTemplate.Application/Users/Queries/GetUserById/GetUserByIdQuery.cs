using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Users.Queries.GetUserById;

/// <summary>
/// Query to get a user by ID
/// </summary>
public class GetUserByIdQuery : IRequest<UserDto?>
{
    /// <summary>
    /// User ID to retrieve
    /// </summary>
    public Guid Id { get; set; }
}
