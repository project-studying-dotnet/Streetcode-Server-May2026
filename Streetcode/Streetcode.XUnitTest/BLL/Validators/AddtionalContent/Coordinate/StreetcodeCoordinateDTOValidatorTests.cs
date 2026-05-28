using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Validators.AdditionalContent.Coordinate;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Coordinate
{
    public class StreetcodeCoordinateDTOValidatorTests
    {
        private readonly StreetcodeCoordinateDTOValidator _validator;

        public StreetcodeCoordinateDTOValidatorTests()
        {
            _validator = new StreetcodeCoordinateDTOValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var dto = new StreetcodeCoordinateDTO { StreetcodeId = invalidId };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_StreetcodeId_Is_Greater_Than_Zero()
        {
            var dto = new StreetcodeCoordinateDTO { StreetcodeId = 1 };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Have_Error_For_Latitude_Because_Of_Included_CoordinateDTOValidator()
        {
            var dto = new StreetcodeCoordinateDTO
            {
                StreetcodeId = 1,
                Latitude = 110.5m 
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Latitude);
        }

        [Fact]
        public void Should_Not_Have_Any_Validation_Errors_When_DTO_Is_Fully_Valid()
        {
            var dto = new StreetcodeCoordinateDTO
            {
                StreetcodeId = 10,
                Latitude = 50.45m, 
                Longtitude = 30.52m
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}