
namespace AgriTech.Infrastructure.Events;

public sealed class UserDomainEventHandler(IUserEventStore userEventStore, IIdentityService identityService) : IDomainEventHandler<UserDomainEvent>
{
    private readonly IIdentityService identityService = identityService;
    private readonly IUserEventStore userEventStore = userEventStore;

    public async Task Handle(UserDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var userEvent = domainEvent.UserEvent;
        userEvent.Location = identityService.GetIpAddress().ToString();
        await userEventStore.Save(userEvent, cancellationToken);
    }
}