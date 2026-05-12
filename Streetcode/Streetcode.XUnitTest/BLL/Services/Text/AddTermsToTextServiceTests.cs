using FluentAssertions;
using Moq;
using Streetcode.BLL.Services.Text;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.Text
{
    public class AddTermsToTextServiceTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly AddTermsToTextService service;

        public AddTermsToTextServiceTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.service = new AddTermsToTextService(this.repositoryWrapperMock.Object);
        }

        [Fact]
        public async Task AddTermsTag_ReturnsNull_WhenInputIsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = await this.service.AddTermsTag(input!);

            // Assert
            result.Should().BeNull();
        }
    }
}
