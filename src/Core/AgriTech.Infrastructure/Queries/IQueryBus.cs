namespace AgriTech.Infrastructure.Queries;

public interface IQueryBus
{
    Task<TResponse> Send<TQuery, TResponse>(TQuery query) where TQuery : IQuery<TResponse>;
}