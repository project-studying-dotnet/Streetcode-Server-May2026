using FluentAssertions;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.AdditionalContent.Coordinates.Types;

public class StreetcodeCoordinateTests
{
    private const int StreetcodeId = 10;

    [Fact]
    public void Properties_ShouldSetValuesCorrectly()
    {
        var coordinate = new StreetcodeCoordinate
        {
            StreetcodeId = StreetcodeId,
            Streetcode = null,
            StatisticRecord = null!,
        };

        coordinate.StreetcodeId.Should().Be(StreetcodeId);
        coordinate.Streetcode.Should().BeNull();
        coordinate.StatisticRecord.Should().BeNull();
    }
}