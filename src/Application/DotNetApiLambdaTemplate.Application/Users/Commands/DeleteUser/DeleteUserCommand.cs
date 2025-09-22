using MediatR;

namespace DotNetApiLambdaTemplate.Application.Users.Commands.DeleteUser;

/// <summary>
/// Command to delete a user
/// </summary>
public class DeleteUserCommand : IRequest
{
    /// <summary>
    /// User ID to delete
    /// </summary>
    public Guid Id { get; set; }
}
