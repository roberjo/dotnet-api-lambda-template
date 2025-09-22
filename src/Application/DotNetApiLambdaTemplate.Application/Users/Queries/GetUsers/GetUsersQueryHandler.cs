using AutoMapper;
using MediatR;
using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.DTOs;
using DotNetApiLambdaTemplate.Domain.Enums;

namespace DotNetApiLambdaTemplate.Application.Users.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PaginatedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // Parse role if provided
        UserRole? role = null;
        if (!string.IsNullOrWhiteSpace(request.Role) && Enum.TryParse<UserRole>(request.Role, true, out var parsedRole))
        {
            role = parsedRole;
        }

        var users = await _userRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            role,
            request.IsActive,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var totalCount = await _userRepository.GetCountAsync(
            request.SearchTerm,
            role,
            request.IsActive,
            cancellationToken);

        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return new PaginatedResult<UserDto>(userDtos, request.PageNumber, request.PageSize, totalCount);
    }
}
