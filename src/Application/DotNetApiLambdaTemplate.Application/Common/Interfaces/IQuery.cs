using MediatR;

namespace DotNetApiLambdaTemplate.Application.Common.Interfaces;

/// <summary>
/// Marker interface for queries in the CQRS pattern
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public interface IQuery<out TResult> : IRequest<TResult>
{
}
