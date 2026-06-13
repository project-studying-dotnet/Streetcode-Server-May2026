using System.Collections.Generic;
using FluentAssertions;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Specifications.Streetcode;

public class PublishedArtSearchSpecificationTests
{
    [Fact]
    public void Constructor_SetsIncludesAndCriteria()
    {
        var spec = new PublishedArtSearchSpecification();

        spec.Includes.Should().NotBeNull();
        spec.Criteria.Should().NotBeNull();

        var criteriaFunc = spec.Criteria!.Compile();

        var validArt = new Art
        {
            StreetcodeArts = new List<StreetcodeArt>
            {
                new StreetcodeArt
                {
                    Streetcode = new StreetcodeContent { Status = StreetcodeStatus.Published }
                }
            }
        };

        var invalidArtDraft = new Art
        {
            StreetcodeArts = new List<StreetcodeArt>
            {
                new StreetcodeArt
                {
                    Streetcode = new StreetcodeContent { Status = StreetcodeStatus.Draft }
                }
            }
        };

        var invalidArtNull = new Art
        {
            StreetcodeArts = new List<StreetcodeArt>
            {
                new StreetcodeArt
                {
                    Streetcode = null
                }
            }
        };

        var emptyArt = new Art
        {
            StreetcodeArts = new List<StreetcodeArt>()
        };

        criteriaFunc(validArt).Should().BeTrue();
        criteriaFunc(invalidArtDraft).Should().BeFalse();
        criteriaFunc(invalidArtNull).Should().BeFalse();
        criteriaFunc(emptyArt).Should().BeFalse();
    }
}