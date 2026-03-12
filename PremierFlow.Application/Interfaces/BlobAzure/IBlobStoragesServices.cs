

namespace PremierFlow.Application.Interfaces.BlobAzure
{
    public interface IBlobStoragesServices
    {
        Task<string> UploadAsync(byte[] fileBytes, string blobName, string contentType);
        Task<bool> DeleteFileAsync(string blobName);
        Task<Stream> GetFileAsync(string blobName);
    }
}