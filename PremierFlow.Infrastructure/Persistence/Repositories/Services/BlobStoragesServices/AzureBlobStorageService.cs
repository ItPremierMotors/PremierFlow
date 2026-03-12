using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using PremierFlow.Application.Interfaces.BlobAzure;

public class AzureBlobStorageService : IBlobStoragesServices
{
    private readonly BlobContainerClient _container;

    public AzureBlobStorageService(IConfiguration config)
    {
        var connectionString = config["AzureStorage:ConnectionString"];
        var containerName = config["AzureStorage:ContainerName"];
        _container = new BlobContainerClient(connectionString, containerName);
        _container.CreateIfNotExists();
    }

    public async Task<string> UploadAsync(byte[] fileBytes, string blobName, string contentType)
    {
        var blob = _container.GetBlobClient(blobName);
        using var stream = new MemoryStream(fileBytes);
        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        });
        return blob.Uri.ToString();
    }
    public async Task<bool> DeleteFileAsync(string blobName)
    {
        var blob = _container.GetBlobClient(blobName);
        var response = await blob.DeleteIfExistsAsync();
        return response.Value;
    }


   
    public Task<Stream> GetFileAsync(string blobName)
    {
        throw new NotImplementedException();
    }
}
