using Microsoft.EntityFrameworkCore;
using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Infrastructure.Data;

namespace DotNetApiLambdaTemplate.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, UserRole? role = null, bool? isActive = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => !u.IsDeleted);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.Name.FirstName.Contains(searchTerm) ||
                u.Name.LastName.Contains(searchTerm) ||
                u.Email.Value.Contains(searchTerm));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var isDescending = sortDirection?.ToLower() == "desc";

            query = sortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(u => u.Name.FirstName) : query.OrderBy(u => u.Name.FirstName),
                "email" => isDescending ? query.OrderByDescending(u => u.Email.Value) : query.OrderBy(u => u.Email.Value),
                "role" => isDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderBy(u => u.CreatedAt)
            };
        }
        else
        {
            query = query.OrderBy(u => u.CreatedAt);
        }

        // Apply pagination
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(string? searchTerm = null, UserRole? role = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => !u.IsDeleted);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.Name.FirstName.Contains(searchTerm) ||
                u.Name.LastName.Contains(searchTerm) ||
                u.Email.Value.Contains(searchTerm));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
    }
}
