namespace AgriTech;

public class SearchCosmosDbSqlService<TEntity> : CosmosBaseRepository, ISearchService<TEntity> where TEntity : BaseDto
{
    private readonly IDistributedCacheRepository cacheRepository;
    private readonly IUniqueCodeGeneratorService uniqueCodeGeneratorService;
    public SearchCosmosDbSqlService(
        IOptions<AzureCosmosDbSettings> cosmosDbOptions,
        CosmosClientFactory cosmosClient,
        IDistributedCacheRepository distributedCacheRepository,
        IUniqueCodeGeneratorService uniqueCodeGeneratorService) : base(
        cosmosDbOptions, cosmosClient)
    {
        this.uniqueCodeGeneratorService = uniqueCodeGeneratorService;
        cacheRepository = distributedCacheRepository;
    }

    public async Task<SearchMetadataDto> GetSearchMetadata(string requestedPageToken, SearchPartFieldDto searchPart, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(requestedPageToken))
        {
            var queryHistory = await cacheRepository.GetAsync<QueryHistoryRecords>(requestedPageToken, cancellationToken);
            if (queryHistory == null)
                return null;
            return new SearchMetadataDto
            {
                MaxRecords = queryHistory.maxRecords,
                PageNumber = queryHistory.pageNumber,
                TotalRecords = queryHistory.totalRecords
            };
        }
        SearchMetadataDto searchMetadata = new();
        var container = await GetCosmosContainer(false, cancellationToken);
        var sqlQuery = new QueryDefinition($"SELECT VALUE Count(1) From r {searchPart.WhereFilter}");
        using var iterator = container.GetItemQueryIterator<int>(sqlQuery);
        var countResponse = await iterator.ReadNextAsync(cancellationToken);
        searchMetadata.MaxRecords = searchPart.MaxRecords;
        searchMetadata.TotalRecords += countResponse.Sum();
        searchMetadata.PageNumber = 1;
        return searchMetadata;
    }

    public async IAsyncEnumerable<TEntity> Search(SearchPartFieldDto searchPart, SearchMetadataDto searchMetadata, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var sqlQuery = new QueryDefinition(searchPart.GetQuery());
        using var feedIterator = container.GetItemQueryIterator<TEntity>(sqlQuery,
            requestOptions: new QueryRequestOptions
            {
                MaxConcurrency = -1,
                MaxItemCount = searchPart.MaxRecords
            });

        var response = await feedIterator.ReadNextAsync(cancellationToken);
        foreach (var item in response)
        {
            yield return item;
            searchMetadata.PageRecordCount++;
        }

        if (response.Count > 0)
        {
            var token = response.ContinuationToken;
            searchMetadata.PreviousPageToken = null;
            searchMetadata.NextPageToken = uniqueCodeGeneratorService.GetUniqueCode();
            await cacheRepository.SetEntityAsync(
                searchMetadata.NextPageToken,
                new QueryHistoryRecords(sqlQuery.QueryText, token, searchPart.MaxRecords, 1, searchMetadata.TotalRecords),
                new TimeSpan(7, 0, 0, 0), cancellationToken);
        }
    }

    public async IAsyncEnumerable<TEntity> Search(string requestedPageToken, SearchMetadataDto searchMetadata, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var queryHistory = await cacheRepository.GetAsync<QueryHistoryRecords>(requestedPageToken, cancellationToken);
        if (queryHistory == null)
            yield break;
        var container = await GetCosmosContainer(false, cancellationToken);
        using var iterator = container.GetItemQueryIterator<TEntity>(new QueryDefinition(queryHistory.query), continuationToken: queryHistory.continuationToken, new QueryRequestOptions
        {
            MaxConcurrency = -1,
            MaxItemCount = queryHistory.maxRecords
        });

        FeedResponse<TEntity> response = await iterator.ReadNextAsync(cancellationToken);
        foreach (var item in response)
        {
            yield return item;
            searchMetadata.PageRecordCount++;
        }

        searchMetadata.PreviousPageToken = requestedPageToken;
        searchMetadata.TotalRecords = queryHistory.totalRecords;
        searchMetadata.MaxRecords = queryHistory.maxRecords;
        searchMetadata.PageNumber = queryHistory.pageNumber + 1;

        if (response.Count == 0)
            yield break;

        var token = response.ContinuationToken;
        if (string.IsNullOrEmpty(queryHistory.nextToken))
        {
            queryHistory.nextToken = uniqueCodeGeneratorService.GetUniqueCode();
            await cacheRepository.SetEntityAsync(requestedPageToken, queryHistory, new TimeSpan(7, 0, 0, 0), cancellationToken);
        }
        searchMetadata.NextPageToken = queryHistory.nextToken;
        await cacheRepository.SetEntityAsync(searchMetadata.NextPageToken,
            new QueryHistoryRecords(queryHistory.query, token, queryHistory.maxRecords, searchMetadata.PageNumber, searchMetadata.TotalRecords),
            new TimeSpan(7, 0, 0, 0), cancellationToken);

    }
    public record TempRecordsDto : BaseDto;

    public async IAsyncEnumerable<string> SearchId(string requestedPageToken, SearchMetadataDto searchMetadata, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(requestedPageToken))
            requestedPageToken = await requestedPageToken.Decompress(cancellationToken);
        var instance = Activator.CreateInstance<TEntity>();
        var entityDiscriminator = typeof(TEntity).GetProperty("Discriminator").GetValue(instance);
        var entityName = entityDiscriminator.ToString();
        var sqlQuery = string.Format(CultureInfo.InvariantCulture, "SELECT Value r.id FROM root r WHERE r.Discriminator='{0}' ", entityName);
        var container = await GetCosmosContainer(false, cancellationToken);
        using var iterator = container.GetItemQueryIterator<string>(new QueryDefinition(sqlQuery), continuationToken: requestedPageToken, new QueryRequestOptions
        {
            MaxConcurrency = -1,
            MaxItemCount = 10
        });

        FeedResponse<string> response = await iterator.ReadNextAsync(cancellationToken);
        foreach (var item in response)
        {
            yield return item;
            searchMetadata.PageRecordCount++;
        }
        searchMetadata.PreviousPageToken = requestedPageToken;
        searchMetadata.MaxRecords = 10;
        if (response.Count > 0)
            searchMetadata.NextPageToken = await response.ContinuationToken.Compress(cancellationToken);
    }
}
