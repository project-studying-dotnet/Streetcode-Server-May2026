using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.Services.BlobStorageService;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.BlobStorageService
{
    public class UpdateFileInStorageTests : IDisposable
    {
        private readonly string _testBlobPath;
        private readonly BlobService _blobService;

        public UpdateFileInStorageTests()
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
        public void UpdateFileInStorage_WhenOldFileExists_DeletesOldFileAndCreatesNewOne()
        {
            var oldFileName = "oldFile.txt";
            File.WriteAllText($"{_testBlobPath}{oldFileName}", "Old Data");
            var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("New Data"));

            var newHashName = _blobService.UpdateFileInStorage(oldFileName, base64Content, "newFile", "txt");

            File.Exists($"{_testBlobPath}{oldFileName}").Should().BeFalse();
            File.Exists($"{_testBlobPath}{newHashName}.txt").Should().BeTrue();
        }

        [Fact]
        public void UpdateFileInStorage_WhenOldFileDoesNotExist_StillCreatesNewFileAndReturnsHashName()
        {
            var nonExistingOldFile = "nonExisting.txt";
            var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("New Data"));

            var newHashName = _blobService.UpdateFileInStorage(nonExistingOldFile, base64Content, "newFile", "txt");

            newHashName.Should().NotBeNullOrEmpty();
            File.Exists($"{_testBlobPath}{newHashName}.txt").Should().BeTrue();
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