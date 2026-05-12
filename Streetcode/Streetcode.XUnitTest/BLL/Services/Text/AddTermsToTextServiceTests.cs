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
        public async Task AddTermsTag_ReturnsNull_WhenInputIsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = await this.service.AddTermsTag(input!);

            // Assert
            result.Should().BeNull();
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
    }
}
