namespace AgriTech;

public interface IRepositoryStream
{
    Task<string> GetDocument(string id, CancellationToken cancellationToken);
}
