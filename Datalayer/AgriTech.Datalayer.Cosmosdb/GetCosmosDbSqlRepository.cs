namespace AgriTech;

public partial class CosmosDbSqlRepository : CosmosBaseRepository, IRepository
{
    public async Task<double> GetSum(string queryFilter, CancellationToken cancellationToken)
    {
        var requestOptions = new QueryRequestOptions
        {
            MaxConcurrency = -1,
            PopulateIndexMetrics = true,
        };
        var container = await GetCosmosContainer(false, cancellationToken);
        using FeedIterator<double> iterator = container.GetItemQueryIterator<double>(new QueryDefinition(queryFilter), requestOptions: requestOptions);
        var countResponse = await iterator.ReadNextAsync(cancellationToken);
        var totalRecords = countResponse.Sum(item => item);
        return totalRecords;
    }
    public async Task<int> GetTotalCount<TEntity>(string queryFilter, CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var requestOptions = new QueryRequestOptions
        {
            MaxConcurrency = -1,
            PopulateIndexMetrics = true
        };
        var container = await GetCosmosContainer(false, cancellationToken);
        using var iterator = container.GetItemQueryIterator<int>(GetCountQueryDefinitation<TEntity>(queryFilter),
            requestOptions: requestOptions);
        var countResponse = await iterator.ReadNextAsync(cancellationToken);
        var totalRecords = countResponse.Sum(item => item);
        return totalRecords;
    }
    public async Task<int> GetTotalCount<TEntity>(CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var query = container.GetItemLinqQueryable<TEntity>(true);
        var entityName = GetDiscriminator<TEntity>();
        return query.Where(item => item.Discriminator == entityName).Count();
    }
    private string GetDiscriminator<TEntity>() where TEntity : BaseDto => Activator.CreateInstance<TEntity>().Discriminator;

    public async Task<int> GetTotalCount<TEntity>(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var query = container.GetItemLinqQueryable<TEntity>(true);
        var entityName = GetDiscriminator<TEntity>();
        return query.Where(predicate).Where(item => item.Discriminator == entityName).Count();
    }


    public async Task<TEntity> GetItem<TEntity>(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var query = container.GetItemLinqQueryable<TEntity>(true);
        var entityName = GetDiscriminator<TEntity>();
        var filter = query.Where(predicate).Where(item => item.Discriminator == entityName);
        TEntity response = null;
        foreach (var item in filter)
        {
            response = item;
            break;
        }

        return response;
    }
    public async Task<TEntity> Get<TEntity>(string id, string partitionKey, CancellationToken cancellationToken) where TEntity : BaseDto
    {
        try
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(partitionKey))
                return null;
            var container = await GetCosmosContainer(false, cancellationToken);
            var itemResponse =
                await container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
            return itemResponse.Resource;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<TEntity> Get<TEntity>(string id, CancellationToken cancellationToken) where TEntity : BaseDto
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return null;
            var container = await GetCosmosContainer(false, cancellationToken);
            var itemResponse =
                await container.ReadItemAsync<TEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return itemResponse.Resource;
        }
        catch (Microsoft.Azure.Cosmos.CosmosException e)
        {
            return null;
        }
    }

    public async IAsyncEnumerable<TEntity> GetAll<TEntity>(CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var sqlQuery = GetDescriptorQueryDefinitation<TEntity>();
        var container = await GetCosmosContainer(false, cancellationToken);
        using var feedIterator = container.GetItemQueryIterator<TEntity>(sqlQuery,
            requestOptions: new QueryRequestOptions
            {
                MaxConcurrency = -1
            });
        while (feedIterator.HasMoreResults)
        {
            var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);

            foreach (var responseItem in feedResponse)
                yield return responseItem;
        }
    }

    public async IAsyncEnumerable<TEntity> GetAll<TEntity>(string queryString, CancellationToken cancellationToken)
        where TEntity : BaseDto
    {
        var sqlQuery = GetDescriptorQueryDefinitation<TEntity>(queryString);
        var container = await GetCosmosContainer(false, cancellationToken);
        using var feedIterator = container.GetItemQueryIterator<TEntity>(sqlQuery,
            requestOptions: new QueryRequestOptions
            {
                MaxConcurrency = -1
            });

        while (feedIterator.HasMoreResults)
        {
            var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
            foreach (var responseItem in feedResponse)
                yield return responseItem;
        }
    }

    public async IAsyncEnumerable<TEntity> Get<TEntity, TKey>(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TKey>> keySelector, bool isDescending, CancellationToken cancellationToken)
        where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var query = container.GetItemLinqQueryable<TEntity>();

        var instance = Activator.CreateInstance<TEntity>();
        var entityDiscriminator = instance.Discriminator;
        var orderdQuery = predicate == null ? query : query.Where(predicate);
        orderdQuery = isDescending ? orderdQuery.OrderByDescending(keySelector) : orderdQuery.OrderBy(keySelector);
        var feedIterator = orderdQuery.Where(item => item.Discriminator == entityDiscriminator).ToFeedIterator();
        while (feedIterator.HasMoreResults)
        {
            var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
            foreach (var responseItem in feedResponse)
                yield return responseItem;
        }
    }

    public async IAsyncEnumerable<TEntity> GetOrderedBy<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector,
        bool isDescending, [EnumeratorCancellation] CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var query = container.GetItemLinqQueryable<TEntity>();
        var entityName = GetDiscriminator<TEntity>();
        var orderdQuery = query.Where(item => item.Discriminator == entityName);
        orderdQuery = isDescending ? orderdQuery.OrderByDescending(keySelector) : orderdQuery.OrderBy(keySelector);
        var feedIterator = orderdQuery.ToFeedIterator();
        while (feedIterator.HasMoreResults)
        {
            var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
            foreach (var responseItem in feedResponse)
                yield return responseItem;
        }
    }

    public async IAsyncEnumerable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var query = container.GetItemLinqQueryable<TEntity>();
        var instance = Activator.CreateInstance<TEntity>();
        var entityDiscriminator = instance.Discriminator;
        var feedIterator = query.Where(predicate).Where(item => item.Discriminator == entityDiscriminator).ToFeedIterator();
        while (feedIterator.HasMoreResults)
            foreach (var responseItem in await feedIterator.ReadNextAsync(cancellationToken))
                yield return responseItem;
    }

    public async IAsyncEnumerable<TEntity> GetPaged<TEntity>(PagedRequestRecords<TEntity> pagedRecords,
        string queryFilter, string orderSequence, CancellationToken cancellationToken, int maxItem = MaxItemCount) where TEntity : BaseDto
    {
        pagedRecords.Response.TotalRecords = await GetTotalCount<TEntity>(queryFilter, cancellationToken);

        var continuationToken = await pagedRecords.RequestedPageToken.Decompress(cancellationToken);
        var container = await GetCosmosContainer(false, cancellationToken);
        using var resultSet = container.GetItemQueryIterator<TEntity>(
            GetDescriptorQueryDefinitation<TEntity>(queryFilter + orderSequence),
            continuationToken,
            new QueryRequestOptions
            {
                MaxItemCount = maxItem,
                MaxConcurrency = -1
                //   PopulateIndexMetrics = true
            });
        var items = await resultSet.ReadNextAsync(cancellationToken);
        pagedRecords.Response.CurrentPageToken = pagedRecords.RequestedPageToken;
        pagedRecords.Response.NextPageToken = await items.ContinuationToken.Compress(cancellationToken);
        pagedRecords.Response.PageSize = MaxItemCount;
        pagedRecords.Response.TotalPages = pagedRecords.Response.TotalRecords / MaxItemCount;
        foreach (var item in items)
            yield return item;
    }

    public async IAsyncEnumerable<TEntity> GetEntities<TEntity>(string queryFilter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        using var feedIterator = container.GetItemQueryIterator<TEntity>(new QueryDefinition(queryFilter));
        var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
        foreach (var responseItem in feedResponse)
            yield return responseItem;
    }


}
