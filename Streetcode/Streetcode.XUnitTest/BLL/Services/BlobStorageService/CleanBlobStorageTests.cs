using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.Services.BlobStorageService
{
    public class CleanBlobStorageTests : IDisposable
    {
        private readonly string _testBlobPath;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly BlobService _blobService;

        public CleanBlobStorageTests()
        {
            _testBlobPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "/");
            Directory.CreateDirectory(_testBlobPath);

            var optionsMock = new Mock<IOptions<BlobEnvironmentVariables>>();
            optionsMock.Setup(o => o.Value).Returns(new BlobEnvironmentVariables
            {
                BlobStoreKey = "12345678901234567890123456789012",
                BlobStorePath = _testBlobPath
            });

            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

            _blobService = new BlobService(optionsMock.Object, _repositoryWrapperMock.Object);
        }

        [Fact]
        public async Task CleanBlobStorage_DeletesFiles_ThatAreNotInDatabase()
        {
            var activeImageBlob = "active-image";
            var activeAudioBlob = "active-audio";
            var orphanBlob = "orphan-file";

            File.WriteAllText($"{_testBlobPath}{activeImageBlob}.jpg", "data");
            File.WriteAllText($"{_testBlobPath}{activeAudioBlob}.mp3", "data");
            File.WriteAllText($"{_testBlobPath}{orphanBlob}.txt", "data");

            var images = new List<Image> { new Image { BlobName = $"{activeImageBlob}.jpg" } };
            var audios = new List<Audio> { new Audio { BlobName = $"{activeAudioBlob}.mp3" } };

            _repositoryWrapperMock.Setup(r => r.ImageRepository.GetAllAsync(null, null)).ReturnsAsync(images);
            _repositoryWrapperMock.Setup(r => r.AudioRepository.GetAllAsync(null, null)).ReturnsAsync(audios);

            await _blobService.CleanBlobStorage();

            File.Exists($"{_testBlobPath}{activeImageBlob}.jpg").Should().BeTrue();
            File.Exists($"{_testBlobPath}{activeAudioBlob}.mp3").Should().BeTrue();
            File.Exists($"{_testBlobPath}{orphanBlob}.txt").Should().BeFalse();
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