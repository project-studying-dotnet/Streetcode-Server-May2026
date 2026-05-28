using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Team; 
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.Validators.Team.Position.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Team.Position.Create
{
    public class CreatePositionQueryValidatorTests
    {
        private readonly CreatePositionQueryValidator _validator;

        public CreatePositionQueryValidatorTests()
        {
            _validator = new CreatePositionQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Position_Is_Null()
        {
            var query = new CreatePositionQuery(null!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.position)
                  .WithErrorMessage("Position is required");
        }

        [Fact]
        public void Should_Have_Error_When_PositionDto_Is_Invalid()
        {
            var invalidDto = new PositionDTO { Position = string.Empty };
            var query = new CreatePositionQuery(invalidDto);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.position.Position);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Position_Is_Valid()
        {
            var validDto = new PositionDTO { Position = "Valid Position" };
            var query = new CreatePositionQuery(validDto);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}