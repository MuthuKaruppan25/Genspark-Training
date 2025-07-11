using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;

public class BlobService : IBlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration config)
    {
        var sasUrl = config["AzureBlob:ConnectionSasUrl"];
        _containerClient = new BlobContainerClient(new Uri(sasUrl!));
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        var headers = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = headers });

        return blobClient.Uri.ToString();
    }
}
