using MediatR;

namespace DotNetApiLambdaTemplate.Application.Common.Interfaces;

/// <summary>
/// Interface for query handlers in the CQRS pattern
/// </summary>
/// <typeparam name="TQuery">The type of query to handle</typeparam>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
}
