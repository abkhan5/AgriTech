using MediatR;

namespace AgriTech.Infrastructure.Queries;

public interface IQuery : IRequest
{

}
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

public interface IQuery<TRequest, TResponse> : IRequest<TResponse>
{
}

public interface IQueryStream<out TResponse> : IStreamRequest<TResponse>
{
}

public interface IPagedQueryStream<TResponse> : IStreamRequest<TResponse> where TResponse : class
{
    public PagedRequestRecords<TResponse> Request { get; set; }
}

public static class MediatorExtensions
{
    public static IAsyncEnumerable<TResponse> CreateStreamRequest<TRequest, TResponse>(this IMediator mediator,
        TRequest request, CancellationToken cancellationToken)
        where TRequest : class, IPagedQueryStream<TResponse>, new()
        where TResponse : class, new()
    {
        return mediator.CreateStream(new TRequest(), cancellationToken);
    }
}