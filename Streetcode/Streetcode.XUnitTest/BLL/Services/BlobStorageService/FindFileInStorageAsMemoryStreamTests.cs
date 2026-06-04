using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.Services.BlobStorageService;
using Xunit;

namespace Streetcode.XUnitTest.Services.BlobStorageService
{
    public class FindFileInStorageAsMemoryStreamTests : IDisposable
    {
        private readonly string _testBlobPath;
        private readonly BlobService _blobService;

        public FindFileInStorageAsMemoryStreamTests()
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
        public void FindFileInStorageAsMemoryStream_WhenFileExists_ReturnsCorrectMemoryStream()
        {
            var fileName = "testfile.txt";
            _blobService.SaveFileInStorageBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello World")), "testfile", "txt");

            var result = _blobService.FindFileInStorageAsMemoryStream(fileName);

            result.Should().NotBeNull();
            Encoding.UTF8.GetString(result.ToArray()).Should().Be("Hello World");
        }

        [Fact]
        public void FindFileInStorageAsMemoryStream_WhenFileNameHasNoExtension_ThrowsIndexOutOfRangeException()
        {
            var invalidFileName = "filenameWithoutDot";

            Action action = () => _blobService.FindFileInStorageAsMemoryStream(invalidFileName);

            action.Should().Throw<IndexOutOfRangeException>();
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