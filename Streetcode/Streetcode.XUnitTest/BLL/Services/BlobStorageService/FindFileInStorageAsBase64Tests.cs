using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.Services.BlobStorageService;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Streetcode.XUnitTest.Services.BlobStorageService
{
    public class BlobServiceFindFileInStorageAsBase64Tests : IDisposable
    {
        private readonly string _testBlobPath;
        private readonly BlobService _blobService;

        public BlobServiceFindFileInStorageAsBase64Tests()
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
        public void FindFileInStorageAsBase64_WhenFileNameHasNoExtension_ReturnsEmptyString()
        {
            var invalidFileName = "filenameWithoutDot";

            var result = _blobService.FindFileInStorageAsBase64(invalidFileName);

            result.Should().BeEmpty();
        }

        [Fact]
        public void FindFileInStorageAsBase64_WhenFileExists_ReturnsCorrectBase64String()
        {
            var fileName = "image.png";
            var rawData = "FakeImageBytes";
            var expectedBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawData));

            _blobService.SaveFileInStorageBase64(expectedBase64, "image", "png");

            var result = _blobService.FindFileInStorageAsBase64(fileName);

            result.Should().Be(expectedBase64);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testBlobPath))
            {
                Directory.Delete(_testBlobPath, true);
            }
        }
    }
}