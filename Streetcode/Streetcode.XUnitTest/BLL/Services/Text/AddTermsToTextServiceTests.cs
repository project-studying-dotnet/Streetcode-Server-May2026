using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Services.Text;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.Text
{
    public class AddTermsToTextServiceTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ITermRepository> termRepositoryMock;
        private readonly AddTermsToTextService service;

        public AddTermsToTextServiceTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.termRepositoryMock = new Mock<ITermRepository>();

            this.repositoryWrapperMock
                .Setup(w => w.TermRepository)
                .Returns(this.termRepositoryMock.Object);

            this.service = new AddTermsToTextService(this.repositoryWrapperMock.Object);
        }

        [Fact]
        public async Task AddTermsTag_ThrowArgumentNullException_WhenInputIsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = async () => await this.service.AddTermsTag(input!);

            // Assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddTermsTag_WrapsWord_WhenDirectTermMatchFound()
        {
            // Arrange
            const string input = "hello";
            var term = new Term { Id = 1, Title = "hello", Description = "test description" };

            this.termRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Term, bool>>>(),
                    It.IsAny<Func<IQueryable<Term>, IIncludableQueryable<Term, object>>?>()))
                .ReturnsAsync(term);

            // Act
            var result = await this.service.AddTermsTag(input);

            // Assert
            result.Should().Contain("<Popover><Term>hello</Term><Desc>test description</Desc></Popover>");
        }

        [Fact]
        public async Task AddTermsTag_WrapsFirstOccurrence_AndSkipsSubsequent()
        {
            // Arrange
            const string input = "hello hello";
            const string expectedTag = "<Popover><Term>hello</Term><Desc>test description</Desc></Popover>";
            var term = new Term { Id = 1, Title = "hello", Description = "test description" };

            this.termRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Term, bool>>>(),
                    It.IsAny<Func<IQueryable<Term>, IIncludableQueryable<Term, object>>?>()))
                .ReturnsAsync(term);

            // Act
            var result = await this.service.AddTermsTag(input);

            // Assert
            result.Should().Contain(expectedTag);
            result.Split("<Popover>").Should().HaveCount(2); // рівно одне входження
        }

        [Fact]
        public async Task AddTermsTag_SkipsHTMLTags_WhenSplittingWords()
        {
            // Arrange
            const string input = "<strong>hello</strong>";
            var term = new Term { Id = 1, Title = "strong", Description = "HTML element" };

            this.termRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Term, bool>>>(),
                    It.IsAny<Func<IQueryable<Term>, IIncludableQueryable<Term, object>>?>()))
                .ReturnsAsync(term);

            // Act
            var result = await this.service.AddTermsTag(input);

            // Assert
            result.Should().NotContain("<Popover><Term>strong</Term>");

            this.termRepositoryMock.Verify(
                repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Term, bool>>>(),
                    It.IsAny<Func<IQueryable<Term>, IIncludableQueryable<Term, object>>?>()),
                Times.Once);
        }
    }
}
