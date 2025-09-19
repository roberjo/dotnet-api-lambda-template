using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.Models;
using DotNetApiLambdaTemplate.Domain.Entities;
using Microsoft.Extensions.Logging;
using IUserRepository = DotNetApiLambdaTemplate.Application.Common.Interfaces.IUserRepository;

namespace DotNetApiLambdaTemplate.Application.Users.Queries.GetUsers;

/// <summary>
/// Handler for the GetUsersQuery
/// </summary>
public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, Result<GetUsersResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<GetUsersResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting users with page {PageNumber}, size {PageSize}", request.PageNumber, request.PageSize);

            // Get users from repository
            var users = await _userRepository.GetAllAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.Role,
                request.IsActive,
                request.SortBy,
                request.SortDirection,
                cancellationToken);

            var totalCount = await _userRepository.GetCountAsync(
                request.SearchTerm,
                request.Role,
                request.IsActive,
                cancellationToken);

            // Map to DTOs
            var userDtos = users.Select(MapToDto).ToList();

            var response = new GetUsersResponse
            {
                Users = userDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            _logger.LogInformation("Successfully retrieved {Count} users", userDtos.Count);

            return Result<GetUsersResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return Result<GetUsersResponse>.Failure($"Failed to get users: {ex.Message}");
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email.Value,
            FirstName = user.Name.FirstName,
            LastName = user.Name.LastName,
            FullName = user.Name.Value,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            PhoneNumber = user.PhoneNumber,
            Department = user.Department,
            JobTitle = user.JobTitle,
            TimeZone = user.TimeZone,
            Language = user.Language,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
