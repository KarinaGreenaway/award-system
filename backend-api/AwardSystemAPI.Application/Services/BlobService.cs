using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AwardSystemAPI.Application.Services;

public interface IBlobService
{
    Task<string> UploadAsync(IFormFile file, string fileName);
}

public class BlobService : IBlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureBlob:ConnectionString"];
        _containerName = configuration["AzureBlob:ContainerName"];
    }

    public async Task<string> UploadAsync(IFormFile file, string fileName)
    {
        var containerClient = new BlobContainerClient(_connectionString, _containerName);
        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        var blob = containerClient.GetBlobClient(fileName);

        await using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, overwrite: true);

        return blob.Uri.ToString();
    }
}
