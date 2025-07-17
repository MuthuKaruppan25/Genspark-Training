using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;

public class BlobService : IBlobService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;


    public BlobService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory

       )
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;

    }

    private async Task<BlobClient> GetBlobClientWithSasAsync(string fileName)
    {
        string functionBaseUrl = _configuration["AzureFunction:GenerateSasBaseUrl"];


        if (string.IsNullOrWhiteSpace(functionBaseUrl))
            throw new InvalidOperationException("Function configuration missing.");

        string requestUrl = $"{functionBaseUrl}/{Uri.EscapeDataString(fileName)}";



        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException("Could not obtain SAS URL.");
        }

        var sasResponse = await response.Content.ReadFromJsonAsync<SasResponse>();
        if (sasResponse == null || string.IsNullOrWhiteSpace(sasResponse.sasUrl))
        {

            throw new InvalidOperationException("Invalid SAS response.");
        }

        return new BlobClient(new Uri(sasResponse.sasUrl));
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var blobClient = await GetBlobClientWithSasAsync(fileName);
        var headers = new BlobHttpHeaders { ContentType = contentType };

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = headers });

        return blobClient.Uri.ToString();
    }
}

public class SasResponse
{
    public string sasUrl { get; set; } = string.Empty;
    public DateTimeOffset expiresOn { get; set; }
}
