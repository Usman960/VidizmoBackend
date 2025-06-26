using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Specialized;

namespace VidizmoBackend.Services
{
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "videos";

        public AzureBlobService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlob:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        // public async Task<string> UploadFileAsync(IFormFile file)
        // {
        //     var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        //     await containerClient.CreateIfNotExistsAsync();
        //     var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

        //     using (var stream = file.OpenReadStream())
        //     {
        //         await blobClient.UploadAsync(stream, true);
        //     }

        //     return blobClient.Uri.ToString();
        // }

        public async Task<Stream> DownloadFileAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException("File not found in blob storage.");

            return await blobClient.OpenReadAsync();
        }

        public async Task<(Stream, string)> StreamFileAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException("Blob not found.");

            var properties = await blobClient.GetPropertiesAsync();
            var stream = await blobClient.OpenReadAsync(new Azure.Storage.Blobs.Models.BlobOpenReadOptions(allowModifications: false));

            return (stream, properties.Value.ContentType ?? "application/octet-stream");
        }

        public async Task DeleteFileAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public string GenerateUploadSasUrl(string originalFileName, out string blobName)
        {
            blobName = Guid.NewGuid() + Path.GetExtension(originalFileName);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            containerClient.CreateIfNotExists();

            var blobClient = containerClient.GetBlockBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create | BlobSasPermissions.Add);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }
    }
}
