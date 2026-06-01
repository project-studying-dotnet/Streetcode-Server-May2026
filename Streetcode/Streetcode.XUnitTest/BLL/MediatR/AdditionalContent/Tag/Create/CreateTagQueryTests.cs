using FluentAssertions;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Xunit;
using Streetcode.BLL.Resources;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.Create;

public class CreateTagQueryTests
{
    [Fact]
    public void Constructor_ShouldSetTag()
    {
        var tagDto = new CreateTagDTO
        {
            Title = ErrorMessages.TagTitleIsEmpty,
        };

        var query = new CreateTagQuery(tagDto);

        query.tag.Should().BeSameAs(tagDto);
        query.tag.Title.Should().Be(ErrorMessages.TagTitleIsEmpty);
    }
}