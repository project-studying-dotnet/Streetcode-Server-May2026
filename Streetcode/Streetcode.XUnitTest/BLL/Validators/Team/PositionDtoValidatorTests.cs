using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Team.Position.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Team.Position.Create
{
    public class PositionDtoValidatorTests
    {
        private readonly PositionDtoValidator _validator;

        public PositionDtoValidatorTests()
        {
            _validator = new PositionDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Position_Is_Empty(string? invalidPosition)
        {
            var dto = new PositionDTO { Position = invalidPosition! };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Position)
                  .WithErrorMessage(ErrorMessages.PositionNameIsRequired);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Position_Is_Valid()
        {
            var dto = new PositionDTO { Position = "Valid Position Name" };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}