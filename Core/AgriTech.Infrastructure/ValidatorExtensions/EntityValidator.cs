
namespace AgriTech;


public static class EntityValidator
{


    public static IRuleBuilderOptions<T, string> IsOwnedAsync<TEntity, T>(this IRuleBuilder<T, string> ruleBuilder, IRepository repository, IIdentityService identity) where TEntity : BaseDto
        => ruleBuilder.NotNull().NotEmpty().
        MustAsync((id, cancellationToken) =>
        repository.IsOwned<TEntity>(id, identity.GetUserIdentity(), cancellationToken)).
        WithErrorCode(AgriTechErrorRegistry.UnAuthorizedError).
        WithMessage(AgriTechErrorRegistry.ErrorCatalog[AgriTechErrorRegistry.UnAuthorizedError]);

    public static IRuleBuilderOptions<T, TEntity> IsNotNullAndOwnedAsync<TEntity, T>(this IRuleBuilder<T, TEntity> ruleBuilder, IRepository repository, IIdentityService identityService) where TEntity : BaseDto
        => ruleBuilder.NotNull().NotEmpty().
        MustAsync((entity, cancellationToken) => repository.IsOwned<TEntity>(entity.Id, identityService.GetUserIdentity(), cancellationToken)).
        WithErrorCode(AgriTechErrorRegistry.UnAuthorizedError).
        WithMessage(AgriTechErrorRegistry.ErrorCatalog[AgriTechErrorRegistry.UnAuthorizedError]);


    public static IRuleBuilderOptions<T, string> EntityExistsAsync<T, TEntity>(this IRuleBuilder<T, string> ruleBuilder, IRepository repository) where TEntity : BaseDto
        => ruleBuilder.
        MustAsync(async (id, cancellationToken) =>
        await repository.Get<TEntity>(id, cancellationToken) != null).
        WithErrorCode(AgriTechErrorRegistry.ResourceNotFound).
        WithMessage(AgriTechErrorRegistry.ErrorCatalog[AgriTechErrorRegistry.ResourceNotFound]);
}
