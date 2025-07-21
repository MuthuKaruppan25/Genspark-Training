using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Streaming_App.Helpers;

public class BlobStorageHelper
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageHelper(IConfiguration config)
    {
        var connectionString = config["AzureBlobStorage:BlobConnectionString"];
        var containerName = config["AzureBlobStorage:ContainerName"];
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadVideoAsync(IFormFile file)
    {
        var blobClient = _containerClient.GetBlobClient($"{Guid.NewGuid()}_{file.FileName}");
        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        return blobClient.Uri.ToString();
    }
    

    public async Task<BlobDownloadStreamingResult?> GetVideoStreamAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        if (!await blobClient.ExistsAsync())
            return null;

        return await blobClient.DownloadStreamingAsync();
    }

    public string ExtractBlobNameFromUrl(string url)
    {
        return Path.GetFileName(new Uri(url).AbsolutePath);
    }
}
