using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.Services.BlobStorageService;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.BlobStorageService
{
    public class SaveFileInStorageTests : IDisposable
    {
        private readonly string _testBlobPath;
        private readonly BlobService _blobService;

        public SaveFileInStorageTests()
        {
            _testBlobPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "/");
            Directory.CreateDirectory(_testBlobPath);

            var optionsMock = new Mock<IOptions<BlobEnvironmentVariables>>();
            optionsMock.Setup(o => o.Value).Returns(new BlobEnvironmentVariables
            {
                BlobStoreKey = "12345678901234567890123456789012",
                BlobStorePath = _testBlobPath
            });

            _blobService = new BlobService(optionsMock.Object);
        }

        [Fact]
        public void SaveFileInStorage_SavesEncryptedFileAndReturnsHashName()
        {
            var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("FileContent"));
            var name = "avatar";
            var extension = "jpg";

            var hashName = _blobService.SaveFileInStorage(base64Content, name, extension);

            hashName.Should().NotBeNullOrEmpty();
            File.Exists($"{_testBlobPath}{hashName}.{extension}").Should().BeTrue();
        }

        [Fact]
        public void SaveFileInStorageBase64_SavesEncryptedFileWithOriginalName()
        {
            var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("FileContent"));
            var name = "specificName";
            var extension = "png";

            _blobService.SaveFileInStorageBase64(base64Content, name, extension);

            File.Exists($"{_testBlobPath}{name}.{extension}").Should().BeTrue();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testBlobPath))
            {
                Directory.Delete(_testBlobPath, true);
            }
            GC.SuppressFinalize(this);
        }
    }
}