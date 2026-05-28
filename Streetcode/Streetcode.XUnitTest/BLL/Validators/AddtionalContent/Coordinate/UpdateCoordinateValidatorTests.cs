using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;
using Streetcode.BLL.Validators.AdditionalContent.Coordinate.Update;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.AddtionalContent.Coordinate
{
    public class UpdateCoordinateValidatorTests
    {
        private readonly UpdateCoordinateValidator _validator;

        public UpdateCoordinateValidatorTests()
        {
            _validator = new UpdateCoordinateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_StreetcodeCoordinate_Is_Null()
        {
            var command = new UpdateCoordinateCommand(null);

            Assert.Throws<NullReferenceException>(() =>
            {
                _validator.TestValidate(command);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_With_Custom_Message_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var coordinateDto = new StreetcodeCoordinateDTO { Id = invalidId };
            var command = new UpdateCoordinateCommand(coordinateDto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeCoordinate.Id)
                  .WithErrorMessage("Coordinate Id must be greater than 0 for update operations.");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Coordinate_And_Id_Are_Valid()
        {

            var validCoordinateDto = new StreetcodeCoordinateDTO { Id = 1 };
            var command = new UpdateCoordinateCommand(validCoordinateDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeCoordinate);
            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeCoordinate.Id);
        }
    }
}