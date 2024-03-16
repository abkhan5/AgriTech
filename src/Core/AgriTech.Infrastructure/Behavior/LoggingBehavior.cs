

namespace AgriTech.Infrastructure.Behavior;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger = logger;
    private readonly IIdentityService identityService = identityService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
                return default;
            return await next();
        }
        catch (Exception e)
        {
            if (cancellationToken.IsCancellationRequested)
                return default;
            if (e.Message == "The operation was canceled.")
                return default;
            var payload = request?.ToJson();
            if (payload == "{}")
                payload = identityService.GetUserIdentity();

            logger.LogCritical(e, "Exception Handled {Name} with request {Request}", typeof(TRequest).Name, payload);
            throw;
        }
    }
}