namespace AgriTech;

public interface IAzureImageService
{
    Task<string> GetAzureOpenAIImage(string imageDescriptor, CancellationToken cancellationToken);
}
