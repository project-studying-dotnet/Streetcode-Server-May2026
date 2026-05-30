using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.AdditionalContent.Coordinate;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Coordinate
{
    public class CoordinateDTOValidatorTests
    {
        private readonly CoordinateDtoValidator _validator;

        public CoordinateDTOValidatorTests()
        {
            _validator = new CoordinateDtoValidator();
        }

        [Theory]
        [InlineData(-90)]
        [InlineData(0)]
        [InlineData(90)]
        public void Should_Not_Have_Error_When_Latitude_Is_Within_Bounds(decimal validLatitude)
        {
            var dto = new StreetcodeCoordinateDTO { Latitude = validLatitude, Longtitude = 0 };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(c => c.Latitude);
        }

        [Theory]
        [InlineData(-90.01)]
        [InlineData(90.01)]
        [InlineData(120)]
        public void Should_Have_Error_When_Latitude_Is_Out_Of_Bounds(decimal invalidLatitude)
        {
            var dto = new StreetcodeCoordinateDTO { Latitude = invalidLatitude, Longtitude = 0 };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(c => c.Latitude)
                  .WithErrorMessage(ErrorMessages.LatitudeMustBeBetweenMinus90And90);
        }

        [Theory]
        [InlineData(-180)]
        [InlineData(0)]
        [InlineData(180)]
        public void Should_Not_Have_Error_When_Longitude_Is_Within_Bounds(decimal validLongitude)
        {
            var dto = new StreetcodeCoordinateDTO { Latitude = 0, Longtitude = validLongitude };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(c => c.Longtitude);
        }

        [Theory]
        [InlineData(-180.01)]
        [InlineData(180.01)]
        [InlineData(250)]
        public void Should_Have_Error_When_Longitude_Is_Out_Of_Bounds(decimal invalidLongitude)
        {
            var dto = new StreetcodeCoordinateDTO { Latitude = 0, Longtitude = invalidLongitude };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(c => c.Longtitude)
                  .WithErrorMessage(ErrorMessages.LongitudeMustBeBetweenMinus180And180);
        }

        [Fact]
        public void Should_Have_Errors_For_All_Fields_When_All_Are_Invalid()
        {
            var dto = new StreetcodeCoordinateDTO { Latitude = 200, Longtitude = 300 };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(c => c.Latitude);
            result.ShouldHaveValidationErrorFor(c => c.Longtitude);
        }

        [Theory]
        [InlineData(-90.00)]
        [InlineData(90.00)]
        public void Should_Not_Have_Error_When_Latitude_Is_On_Edge(decimal edgeLatitude)
        {
            var dto = new StreetcodeCoordinateDTO { Latitude = edgeLatitude, Longtitude = 0 };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(c => c.Latitude);
        }

        [Theory]
        [InlineData(-180.00)]
        [InlineData(180.00)]
        public void Should_Not_Have_Error_When_Longitude_Is_On_Edge(decimal edgeLongitude)
        {
            var dto = new StreetcodeCoordinateDTO { Latitude = 0, Longtitude = edgeLongitude };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(c => c.Longtitude);
        }
    }
}