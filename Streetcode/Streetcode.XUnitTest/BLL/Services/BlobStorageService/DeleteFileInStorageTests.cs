using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.Services.BlobStorageService;
using Xunit;

namespace Streetcode.XUnitTest.Services.BlobStorageService
{
    public class DeleteFileInStorageTests : IDisposable
    {
        private readonly string _testBlobPath;
        private readonly BlobService _blobService;

        public DeleteFileInStorageTests()
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
        public void DeleteFileInStorage_WhenFileExists_RemovesFileFromDisk()
        {
            var fullFileName = "fileToDelete.txt";
            File.WriteAllText($"{_testBlobPath}{fullFileName}", "Goodbye World");

            _blobService.DeleteFileInStorage(fullFileName);

            File.Exists($"{_testBlobPath}{fullFileName}").Should().BeFalse();
        }

        [Fact]
        public void DeleteFileInStorage_WhenFileDoesNotExist_ShouldNotThrowException()
        {
            var nonExistingFile = "ghost.txt";

            Action action = () => _blobService.DeleteFileInStorage(nonExistingFile);

            action.Should().NotThrow();
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