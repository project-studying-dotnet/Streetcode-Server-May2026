using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Interfaces.BlobStorage;

namespace Streetcode.BLL.Services.BlobStorageService;

[ExcludeFromCodeCoverage]
public class AzureBlobService : IBlobService
{
    private readonly BlobContainerClient _container;
    private readonly string _keyCrypt;

    public AzureBlobService(IOptions<AzureBlobEnvironmentVariables> options)
    {
        var client = new BlobServiceClient(options.Value.ConnectionString);

        _container = client.GetBlobContainerClient(options.Value.ContainerName);
        _container.CreateIfNotExists();

        _keyCrypt = options.Value.BlobStoreKey;
    }

    private static string GenerateHash(string value)
    {
        byte[] result = SHA256.HashData(Encoding.UTF8.GetBytes(value));

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

        byte[] encryptedBytes = EncryptBytes(fileBytes);

        using var stream = new MemoryStream(encryptedBytes);

        var blobClient = _container.GetBlobClient(blobName);

        blobClient.Upload(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = GetContentType(extension)
                },
                Conditions = null
            },
            cancellationToken: default);

        return hashName;
    }

    public MemoryStream FindFileInStorageAsMemoryStream(string name)
    {
        byte[] content = DownloadBlob(name);

        return new MemoryStream(content);
    }

    public string FindFileInStorageAsBase64(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        byte[] content = DownloadBlob(name);

        return Convert.ToBase64String(content);
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

    private byte[] DownloadBlob(string blobName)
    {
        var blobClient = _container.GetBlobClient(blobName);

        if (!blobClient.Exists())
        {
            throw new FileNotFoundException(
                $"Blob '{blobName}' was not found.");
        }

        var response = blobClient.DownloadContent();
        byte[] encryptedBytes = response.Value.Content.ToArray();

        return DecryptBytes(encryptedBytes);
    }

    private byte[] EncryptBytes(byte[] fileBytes)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(_keyCrypt);

        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = keyBytes;

        aes.GenerateIV();
        byte[] iv = aes.IV;

        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);

        byte[] encryptedData = new byte[iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(iv, 0, encryptedData, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, encryptedData, iv.Length, encryptedBytes.Length);

        return encryptedData;
    }

    private byte[] DecryptBytes(byte[] encryptedData)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(_keyCrypt);

        byte[] iv = new byte[16];
        Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);

        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = keyBytes;
        aes.IV = iv;

        using ICryptoTransform decryptor = aes.CreateDecryptor();

        return decryptor.TransformFinalBlock(
            encryptedData,
            iv.Length,
            encryptedData.Length - iv.Length);
    }
}