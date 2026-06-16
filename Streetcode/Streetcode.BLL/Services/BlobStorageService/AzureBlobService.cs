using System.Security.Cryptography;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Interfaces.BlobStorage;
using Azure.Storage.Blobs.Models;

namespace Streetcode.BLL.Services.BlobStorageService;

public class AzureBlobService : IBlobService
{
    private readonly BlobContainerClient _container;

    public AzureBlobService(
        IOptions<AzureBlobEnvironmentVariables> options)
    {
        var client = new BlobServiceClient(
            options.Value.ConnectionString);

        _container = client.GetBlobContainerClient(
            options.Value.ContainerName);
    }

    public string SaveFileInStorage(
     string base64,
     string name,
     string extension)
    {
        byte[] fileBytes = Convert.FromBase64String(base64);

        string generatedName = $"{DateTime.UtcNow}{name}"
            .Replace(" ", "_")
            .Replace(".", "_")
            .Replace(":", "_");

        string hashName = GenerateHash(generatedName);

        string blobName = $"{hashName}.{extension}";

        using var stream = new MemoryStream(fileBytes);

        var blobClient = _container.GetBlobClient(blobName);

        blobClient.Upload(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = GetContentType(extension)
                }
            });
        Console.WriteLine($"Azure Blob URL: {blobClient.Uri}");
        return hashName;
    }
    public MemoryStream FindFileInStorageAsMemoryStream(string name)
    {
        byte[] content = DownloadBlob(name);

        return new MemoryStream(content);
    }

    public string FindFileInStorageAsBase64(string name)
    {
        byte[] content = DownloadBlob(name);

        return Convert.ToBase64String(content);
    }
    private byte[] DownloadBlob(string blobName)
    {
        var blobClient = _container.GetBlobClient(blobName);

        if (!blobClient.Exists())
        {
            throw new FileNotFoundException(
                $"Blob '{blobName}' was not found.");
        }

        var response = blobClient.DownloadContent();

        return response.Value.Content.ToArray();
    }

    public void DeleteFileInStorage(string name)
    {
        var blobClient = _container.GetBlobClient(name);

        blobClient.DeleteIfExists();
    }

    public string UpdateFileInStorage(
    string previousBlobName,
    string base64Format,
    string newBlobName,
    string extension)
    {
        DeleteFileInStorage(previousBlobName);

        return SaveFileInStorage(
            base64Format,
            newBlobName,
            extension);
    }

    private static string GenerateHash(string value)
    {
        byte[] result = SHA256.HashData(
            Encoding.UTF8.GetBytes(value));

        return Convert.ToBase64String(result)
            .Replace('/', '_');
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "gif" => "image/gif",
            "svg" => "image/svg+xml",
            "mp3" => "audio/mpeg",
            "wav" => "audio/wav",
            _ => "application/octet-stream"
        };
    }
}