using System;
using System.Collections.Generic;
using FluentAssertions;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Specifications.Streetcode;

public class StreetcodesByTagIdSpecificationTests
{
    [Fact]
    public void Criteria_WhenStatusIsPublishedAndTagMatches_ShouldReturnTrue()
    {
        var spec = new StreetcodesByTagIdSpecification(1);
        var streetcode = new StreetcodeContent
        {
            Status = StreetcodeStatus.Published,
            Tags = new List<Tag> { new Tag { Id = 1, Title = "Test Title" } }
        };

        var compiledCriteria = spec.Criteria!.Compile();
        var result = compiledCriteria(streetcode);

        result.Should().BeTrue();
    }

    [Fact]
    public void Criteria_WhenStatusIsNotPublished_ShouldReturnFalse()
    {
        var spec = new StreetcodesByTagIdSpecification(1);
        var streetcode = new StreetcodeContent
        {
            Status = StreetcodeStatus.Draft,
            Tags = new List<Tag> { new Tag { Id = 1, Title = "Test Title" } }
        };

        var compiledCriteria = spec.Criteria!.Compile();
        var result = compiledCriteria(streetcode);

        result.Should().BeFalse();
    }

    [Fact]
    public void Criteria_WhenTagDoesNotMatch_ShouldReturnFalse()
    {
        var spec = new StreetcodesByTagIdSpecification(1);
        var streetcode = new StreetcodeContent
        {
            Status = StreetcodeStatus.Published,
            Tags = new List<Tag> { new Tag { Id = 2, Title = "Test Title" } }
        };

        var compiledCriteria = spec.Criteria!.Compile();
        var result = compiledCriteria(streetcode);

        result.Should().BeFalse();
    }

    [Fact]
    public void Includes_ShouldNotBeNull()
    {
        var spec = new StreetcodesByTagIdSpecification(1);

        spec.Includes.Should().NotBeNull();
    }
}