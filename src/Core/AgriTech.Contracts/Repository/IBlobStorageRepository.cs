namespace AgriTech;
public enum BlobStoreTypesEnums
{
    Prev,
    AppStore,
    VideoStore,
    AzureAssets,
    ImageStore,
    TableStore,
    UserVideoStore
}
public interface IPrevBlobStorageRepository
{
    IAsyncEnumerable<BlobStorageItem> MigrateProfileImages(string containerName, string prefix = null);

}

public interface IBlobStorageRepository
{
    public void SetStoreAppType(BlobStoreTypesEnums blobStoreTypes);
    Task<byte[]> GetFileAsync(BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task<Stream> GetFileStreamAsync(BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task<Stream> GetFileStreamAsync(string containerName, string fileName, CancellationToken cancellationToken);
    Task<string> SaveAsync(Stream stream, BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task CopyFiles(BlobStoreTypesEnums originSource, string originContainer, string destinationContainer, CancellationToken cancellationToken);
    IAsyncEnumerable<BlobStorageItem> GetAllFiles(BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task<IDictionary<string, string>> GetMetaData(BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task<IDictionary<string, string>> GetMetaData(string containerName, string fileName, CancellationToken cancellationToken);
    Task SaveMetaData(BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task SaveMetaData(string containerName, string fileName, IDictionary<string, string> metadata, CancellationToken cancellationToken);
    Task DeleteAsync(BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task DeleteFolderAsync(BlobStorageItem storageItem, CancellationToken cancellationToken);
    Task DeleteStoreAsync(string containerName, CancellationToken cancellationToken);
    IAsyncEnumerable<BlobStorageItem> GetFileListAsync(CancellationToken cancellationToken);
    Task<AzureStorageSASResult> GetSasToken(BlobStorageItem storageItem, CancellationToken cancellationToken);

}