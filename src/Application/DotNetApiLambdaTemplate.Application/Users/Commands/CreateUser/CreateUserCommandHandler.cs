using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.Models;
using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Services;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using IUserRepository = DotNetApiLambdaTemplate.Application.Common.Interfaces.IUserRepository;

namespace DotNetApiLambdaTemplate.Application.Users.Commands.CreateUser;

/// <summary>
/// Handler for the CreateUserCommand
/// </summary>
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<User>>
{
    private readonly IUserDomainService _userDomainService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserDomainService userDomainService,
        IUserRepository userRepository,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating user with email: {Email}", request.Email);

            // Create value objects
            var email = Email.Create(request.Email);
            var fullName = FullName.Create(request.FirstName, request.LastName);

            // Check if email is available
            var isEmailAvailable = await _userDomainService.IsEmailAvailableAsync(email, cancellationToken);
            if (!isEmailAvailable)
            {
                _logger.LogWarning("Email {Email} is already taken", request.Email);
                return Result<User>.Failure("Email address is already taken");
            }

            // Create the user
            var user = new User(
                Guid.NewGuid(),
                fullName,
                email,
                request.Role,
                request.CreatedBy,
                request.PhoneNumber,
                request.Department,
                request.JobTitle,
                null, // ExternalId
                request.TimeZone,
                request.Language);

            // Save the user
            var savedUser = await _userRepository.AddAsync(user, cancellationToken);

            _logger.LogInformation("Successfully created user with ID: {UserId}", savedUser.Id);

            return Result<User>.Success(savedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", request.Email);
            return Result<User>.Failure($"Failed to create user: {ex.Message}");
        }
    }
}
