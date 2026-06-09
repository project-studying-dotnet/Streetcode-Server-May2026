using System.Collections.Generic;
using FluentAssertions;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Specifications.Streetcode;

public class RelatedFiguresSpecificationTests
{
    [Fact]
    public void Constructor_SetsIncludesAndCriteria()
    {
        var relatedIds = new List<int> { 1, 2, 3 };

        var spec = new RelatedFiguresSpecification(relatedIds);

        spec.Includes.Should().NotBeNull();
        spec.Criteria.Should().NotBeNull();

        var criteriaFunc = spec.Criteria!.Compile();

        var validStreetcode = new StreetcodeContent
        {
            Id = 2,
            Status = StreetcodeStatus.Published
        };

        var invalidDraftStreetcode = new StreetcodeContent
        {
            Id = 2,
            Status = StreetcodeStatus.Draft
        };

        var invalidIdStreetcode = new StreetcodeContent
        {
            Id = 4,
            Status = StreetcodeStatus.Published
        };

        criteriaFunc(validStreetcode).Should().BeTrue();
        criteriaFunc(invalidDraftStreetcode).Should().BeFalse();
        criteriaFunc(invalidIdStreetcode).Should().BeFalse();
    }
}