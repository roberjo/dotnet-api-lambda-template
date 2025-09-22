namespace DotNetApiLambdaTemplate.Application.Common.DTOs;

/// <summary>
/// Represents a paginated result with data and metadata
/// </summary>
/// <typeparam name="T">Type of data items</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// The data items for the current page
    /// </summary>
    public IEnumerable<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a new paginated result
    /// </summary>
    public PaginatedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates an empty paginated result
    /// </summary>
    public static PaginatedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PaginatedResult<T>(new List<T>(), pageNumber, pageSize, 0);
    }
}
