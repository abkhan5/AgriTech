using System.Runtime.CompilerServices;

namespace AgriTech;

public interface ISearchService<TEntity> where TEntity : BaseDto
{

    void SetContainerName(string containerName);
    IAsyncEnumerable<TEntity> Search(string requestedPageToken, SearchMetadataDto serchMetadata, CancellationToken cancellationToken);

    IAsyncEnumerable<TEntity> Search(SearchPartFieldDto searchPart, SearchMetadataDto serchMetadata, CancellationToken cancellationToken);

    Task<SearchMetadataDto> GetSearchMetadata(string requestedToken, SearchPartFieldDto searchPart, CancellationToken cancellationToken);
    IAsyncEnumerable<string> SearchId(string requestedPageToken, SearchMetadataDto searchMetadata, [EnumeratorCancellation] CancellationToken cancellationToken);
}

