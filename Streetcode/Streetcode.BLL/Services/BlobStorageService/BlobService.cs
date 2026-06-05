using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Services.BlobStorageService;

public class BlobService : IBlobService
{
    private readonly BlobEnvironmentVariables _envirovment;
    private readonly string _keyCrypt;
    private readonly string _blobPath;
    private readonly IRepositoryWrapper? _repositoryWrapper;

    public BlobService(IOptions<BlobEnvironmentVariables> environment, IRepositoryWrapper? repositoryWrapper = null)
    {
        _envirovment = environment.Value;
        _keyCrypt = _envirovment.BlobStoreKey;
        _blobPath = _envirovment.BlobStorePath;
        _repositoryWrapper = repositoryWrapper;
    }

    private static string HashFunction(string createdFileName)
    {
        byte[] result = SHA256.HashData(Encoding.UTF8.GetBytes(createdFileName));

        return Convert.ToBase64String(result).Replace('/', '_');
    }
    public MemoryStream FindFileInStorageAsMemoryStream(string name)
    {
        string[] splitedName = name.Split('.');

        byte[] decodedBytes = DecryptFile(splitedName[0], splitedName[1]);

        var image = new MemoryStream(decodedBytes);

        return image;
    }

    public string FindFileInStorageAsBase64(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        string extension = Path.GetExtension(name).TrimStart('.');
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(name);

        if (string.IsNullOrEmpty(extension) || string.IsNullOrEmpty(nameWithoutExtension))
        {
            return string.Empty;
        }

        byte[] decodedBytes = DecryptFile(nameWithoutExtension, extension);

        string base64 = Convert.ToBase64String(decodedBytes);

        return base64;
    }

    public string SaveFileInStorage(string base64, string name, string extension)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);
        string createdFileName = $"{DateTime.Now}{name}"
            .Replace(" ", "_")
            .Replace(".", "_")
            .Replace(":", "_");

        string hashBlobStorageName = HashFunction(createdFileName);

        Directory.CreateDirectory(_blobPath);
        EncryptFile(imageBytes, extension, hashBlobStorageName);

        return hashBlobStorageName;
    }

    public void SaveFileInStorageBase64(string base64, string name, string extension)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);
        Directory.CreateDirectory(_blobPath);
        EncryptFile(imageBytes, extension, name);
    }

    public void DeleteFileInStorage(string name)
    {
        File.Delete($"{_blobPath}{name}");
    }

    public string UpdateFileInStorage(
        string previousBlobName,
        string base64Format,
        string newBlobName,
        string extension)
    {
        DeleteFileInStorage(previousBlobName);

        string hashBlobStorageName = SaveFileInStorage(
        base64Format,
        newBlobName,
        extension);

        return hashBlobStorageName;
    }

    public async Task CleanBlobStorage()
    {
        var base64Files = GetAllBlobNames();

        var existingImagesInDatabase = await _repositoryWrapper!.ImageRepository.GetAllAsync();
        var existingAudiosInDatabase = await _repositoryWrapper.AudioRepository.GetAllAsync();

        List<string> existingMedia = new ();
        existingMedia.AddRange(existingImagesInDatabase
            .Select(img => img.BlobName)
            .Where(name => name != null)!);

        existingMedia.AddRange(existingAudiosInDatabase
            .Select(audio => audio.BlobName)
            .Where(name => name != null)!);

        var filesToRemove = base64Files.Except(existingMedia).ToList();

        foreach (var file in filesToRemove)
        {
            Console.WriteLine($"Deleting {file}...");
            DeleteFileInStorage(file);
        }
    }

    private IEnumerable<string> GetAllBlobNames()
    {
        var paths = Directory.EnumerateFiles(_blobPath);

        return paths.Select(p => Path.GetFileName(p));
    }

    private void EncryptFile(byte[] imageBytes, string type, string name)
    {
        byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(_keyCrypt));
        byte[] iv = new byte[16];
        RandomNumberGenerator.Fill(iv);

        byte[] encryptedBytes;

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.IV = iv;

            using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                encryptedBytes = encryptor.TransformFinalBlock(imageBytes, 0, imageBytes.Length);
            }
        }

        byte[] encryptedData = new byte[iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(iv, 0, encryptedData, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, encryptedData, iv.Length, encryptedBytes.Length);

        File.WriteAllBytes(Path.Combine(_blobPath, $"{name}.{type}"), encryptedData);
    }

    private byte[] DecryptFile(string fileName, string type)
    {
        byte[] encryptedData = File.ReadAllBytes($"{_blobPath}{fileName}.{type}");

        byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(_keyCrypt));

        byte[] iv = new byte[16];
        Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);

        byte[] decryptedBytes;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = iv;

            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                decryptedBytes = decryptor.TransformFinalBlock(encryptedData, iv.Length, encryptedData.Length - iv.Length);
            }
        }

        return decryptedBytes;
    }
}