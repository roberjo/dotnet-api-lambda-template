using MediatR;

namespace DotNetApiLambdaTemplate.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands in the CQRS pattern
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Marker interface for commands that return a result
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>
{
}
