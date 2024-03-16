

namespace AgriTech.Infrastructure.Behavior;

internal sealed class ValidatorBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return default;

        if (!validators.Any())
            return await next();

        List<ValidationFailure> messagesErrors = [];


        await foreach (var errors in GetFailures(request, cancellationToken))
            messagesErrors.AddRange(errors);


        if (messagesErrors.Count != 0)
            throw new ValidationException(messagesErrors);

        return await next();
    }

    private async IAsyncEnumerable<List<ValidationFailure>> GetFailures(TRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var response = await validator.ValidateAsync(request, cancellationToken);
            if (response != null && response.Errors.Count != 0)
                yield return response.Errors;
        }
    }
}