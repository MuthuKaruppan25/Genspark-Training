using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace Company.Function;

public class PortalFunc
{
    private readonly ILogger<PortalFunc> _logger;

    public PortalFunc(ILogger<PortalFunc> logger)
    {
        _logger = logger;
    }

    [Function("PortalFunc")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "generate-sas/{blobName}")] HttpRequestData req,
        string blobName)
    {
        _logger.LogInformation($"Generating SAS for blob: {blobName}");

        string connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
        string containerName = Environment.GetEnvironmentVariable("ContainerName");
        string keyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri");

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(keyVaultUri))
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Missing required configuration settings.");
            return errorResponse;
        }

        // Parse AccountKey
        string accountKey = connectionString.Split(';')
            .FirstOrDefault(p => p.StartsWith("AccountKey=", StringComparison.OrdinalIgnoreCase))
            ?.Substring("AccountKey=".Length);

        if (string.IsNullOrEmpty(accountKey))
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("AccountKey not found in connection string.");
            return errorResponse;
        }

        // Create clients
        var blobServiceClient = new BlobServiceClient(connectionString);
        var accountName = blobServiceClient.AccountName;
        var credential = new StorageSharedKeyCredential(accountName, accountKey);
        var blobClient = blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);

        // Create SAS
        DateTimeOffset expiresOn = DateTimeOffset.UtcNow.AddHours(1);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expiresOn
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);

        // Store SAS in Key Vault
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        var secretToStore = new KeyVaultSecret("muthusasurl", sasUri.ToString())
        {
            Properties = { Tags = { { "ExpiresOn", expiresOn.UtcDateTime.ToString("o") } } }
        };
        await secretClient.SetSecretAsync(secretToStore);

        // Return response
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            sasUrl = sasUri.ToString(),
            expiresOn
        });

        return response;
    }
}
