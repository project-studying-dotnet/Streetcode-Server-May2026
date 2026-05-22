using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.BLL.Validators.AdditionalContent.Coordinate.Create;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.AddtionalContent.Coordinate
{
    public class CreateCoordinateValidatorTests
    {
        private readonly CreateCoordinateValidator _validator;

        public CreateCoordinateValidatorTests()
        {
            _validator = new CreateCoordinateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_StreetcodeCoordinate_Is_Null()
        {
            var command = new CreateCoordinateCommand(null);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeCoordinate);
        }

        [Fact]
        public void Should_Not_Have_NotNull_Error_When_StreetcodeCoordinate_Is_Provided()
        {

            var validCoordinateDto = new StreetcodeCoordinateDTO();
            var command = new CreateCoordinateCommand(validCoordinateDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeCoordinate);
        }
    }
}