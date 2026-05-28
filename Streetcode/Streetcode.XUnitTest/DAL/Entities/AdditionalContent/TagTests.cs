using FluentAssertions;
using Streetcode.DAL.Entities.AdditionalContent;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.AdditionalContent;

public class TagTests
{
    private const int TagId = 1;
    private const string Title = "History";

    [Fact]
    public void Properties_ShouldSetValuesCorrectly()
    {
        var tag = new Tag
        {
            Id = TagId,
            Title = Title,
        };

        tag.Id.Should().Be(TagId);
        tag.Title.Should().Be(Title);

        tag.StreetcodeTagIndices.Should().NotBeNull();
        tag.Streetcodes.Should().NotBeNull();

        tag.StreetcodeTagIndices.Should().BeEmpty();
        tag.Streetcodes.Should().BeEmpty();
    }
}