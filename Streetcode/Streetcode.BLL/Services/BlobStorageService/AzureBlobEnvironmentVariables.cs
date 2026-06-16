using System.Diagnostics.CodeAnalysis;

namespace Streetcode.BLL.Services.BlobStorageService;

[ExcludeFromCodeCoverage]
public class AzureBlobEnvironmentVariables
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}