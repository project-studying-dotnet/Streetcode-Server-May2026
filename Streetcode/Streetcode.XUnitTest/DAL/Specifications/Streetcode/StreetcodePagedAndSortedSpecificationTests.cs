using System;
using FluentAssertions;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Specifications.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Specifications.Streetcode;

public class StreetcodePagedAndSortedSpecificationTests
{
    [Fact]
    public void Constructor_WithNullParameters_SetsOnlyPagination()
    {
        // Arrange
        int page = 2;
        int amount = 10;

        // Act
        var spec = new StreetcodePagedAndSortedSpecification(page, amount, null, null, null);

        // Assert
        spec.Skip.Should().Be(10);
        spec.Take.Should().Be(10);
        spec.Criteria.Should().BeNull();
        spec.OrderBy.Should().BeNull();
        spec.IsDescending.Should().BeFalse();
    }

    [Fact]
    public void ApplyTitle_ValidTitle_SetsCriteria()
    {
        // Act
        var spec = new StreetcodePagedAndSortedSpecification(1, 10, "TestTitle", null, null);

        // Assert
        spec.Criteria.Should().NotBeNull();
    }

    [Fact]
    public void ApplyFilter_InvalidFormat_DoesNotSetCriteria()
    {
        // Act
        var spec = new StreetcodePagedAndSortedSpecification(1, 10, null, null, "InvalidFilterFormat");

        // Assert
        spec.Criteria.Should().BeNull();
    }

    [Fact]
    public void ApplyFilter_ValidFormat_SetsCriteria()
    {
        // Act
        var spec = new StreetcodePagedAndSortedSpecification(1, 10, null, null, "Status:Active");

        // Assert
        spec.Criteria.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Title", false)]
    [InlineData("-Title", true)]
    [InlineData("Index", false)]
    [InlineData("-CreatedAt", true)]
    [InlineData("UpdatedAt", false)]
    [InlineData("-EventStartOrPersonBirthDate", true)]
    public void ApplySort_ValidSortParameters_SetsOrderByAndDirection(string sort, bool expectedIsDescending)
    {
        // Act
        var spec = new StreetcodePagedAndSortedSpecification(1, 10, null, sort, null);

        // Assert
        spec.OrderBy.Should().NotBeNull();
        spec.IsDescending.Should().Be(expectedIsDescending);
    }

    [Fact]
    public void ApplySort_UnknownColumn_SetsOrderByNullAndIsDescendingFalse()
    {
        // Act
        var spec = new StreetcodePagedAndSortedSpecification(1, 10, null, "UnknownColumn", null);

        // Assert
        spec.OrderBy.Should().BeNull();
        spec.IsDescending.Should().BeFalse();
    }
}