namespace Microsoft.Extensions.DependencyInjection;

public static class MediatRExtensions
{
    public static void ConfigureMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            //cfg.AddBehavior<PingPongBehavior>();
            //cfg.AddStreamBehavior<PingPongStreamBehavior>();
            //cfg.AddRequestPreProcessor<PingPreProcessor>();
            //cfg.AddRequestPostProcessor<PingPongPostProcessor>();  
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });
    }
}