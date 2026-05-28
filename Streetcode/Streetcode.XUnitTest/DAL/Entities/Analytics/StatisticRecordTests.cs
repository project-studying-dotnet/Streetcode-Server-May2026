using FluentAssertions;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Entities.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.Analytics;

public class StatisticRecordTests
{
    [Fact]
    public void StatisticRecord_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var streetcode = new StreetcodeContent
        {
            Id = 10,
            Title = "Test Streetcode",
            TransliterationUrl = "test-streetcode",
        };

        var coordinate = new StreetcodeCoordinate
        {
            Id = 20,
            StreetcodeId = 10,
        };

        // Act
        var statisticRecord = new StatisticRecord
        {
            Id = 1,
            QrId = 100,
            Count = 50,
            Address = "Lviv",
            StreetcodeId = 10,
            Streetcode = streetcode,
            StreetcodeCoordinateId = 20,
            StreetcodeCoordinate = coordinate,
        };

        // Assert
        statisticRecord.Id.Should().Be(1);
        statisticRecord.QrId.Should().Be(100);
        statisticRecord.Count.Should().Be(50);
        statisticRecord.Address.Should().Be("Lviv");

        statisticRecord.StreetcodeId.Should().Be(10);
        statisticRecord.Streetcode.Should().BeSameAs(streetcode);

        statisticRecord.StreetcodeCoordinateId.Should().Be(20);
        statisticRecord.StreetcodeCoordinate.Should().BeSameAs(coordinate);
    }

    [Fact]
    public void StatisticRecord_Address_ShouldDefaultToEmptyString()
    {
        // Act
        var statisticRecord = new StatisticRecord();

        // Assert
        statisticRecord.Address.Should().Be(string.Empty);
    }
}