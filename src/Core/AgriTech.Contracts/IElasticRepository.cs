namespace AgriTech;

public interface IElasticRepository<T> where T : BaseDto
{
    Task<T?> Get(string index, T entity, CancellationToken cancellationToken);
    Task Add(string index, T entity, CancellationToken cancellationToken);
}
