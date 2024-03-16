using MediatR;

namespace AgriTech.Infrastructure.Queries;
public interface IQueryHandler<in TQuery> : IRequestHandler<TQuery>
    where TQuery : IQuery
{
}


public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}

public interface IQueryStreamHandler<in TQuery, out TResponse> : IStreamRequestHandler<TQuery, TResponse>
    where TQuery : IStreamRequest<TResponse>
{
}