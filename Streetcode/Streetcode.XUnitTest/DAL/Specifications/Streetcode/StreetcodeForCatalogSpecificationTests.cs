using FluentAssertions;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Specifications.Streetcode;

public class StreetcodeForCatalogSpecificationTests
{
    [Fact]
    public void Constructor_SetsPaginationIncludesAndCriteria()
    {
        var page = 3;
        var count = 15;

        var spec = new StreetcodeForCatalogSpecification(page, count);

        spec.Skip.Should().Be(30);
        spec.Take.Should().Be(15);
        spec.Includes.Should().NotBeNull();
        spec.Criteria.Should().NotBeNull();

        var criteriaFunc = spec.Criteria!.Compile();
        var publishedStreetcode = new StreetcodeContent { Status = StreetcodeStatus.Published };
        var draftStreetcode = new StreetcodeContent { Status = StreetcodeStatus.Draft };

        criteriaFunc(publishedStreetcode).Should().BeTrue();
        criteriaFunc(draftStreetcode).Should().BeFalse();
    }
}