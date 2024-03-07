namespace AgriTech;

public interface IUserEventStore
{
    Task Save(UserEventDto userEvent, CancellationToken cancellationToken);

    // IAsyncEnumerable<UserEventDto> Get(string userId, int? numberOfItems, CancellationToken cancellationToken);
}
