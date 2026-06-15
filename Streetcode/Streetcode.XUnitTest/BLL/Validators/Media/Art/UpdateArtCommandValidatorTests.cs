using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Art.Update;
using Streetcode.BLL.Validators.Media.Art;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.Art
{
    public class UpdateArtCommandValidatorTests
    {
        private readonly UpdateArtCommandValidator _validator = new();

        [Fact]
        public void CommandValidator_Should_Detect_Error_When_Dto_Is_Null()
        {
            var command = new UpdateArtCommand(null!);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ArtDto);
        }

        [Fact]
        public void CommandValidator_Should_Call_Nested_Validator_And_Fail_On_Invalid_Dto()
        {
            var command = new UpdateArtCommand(new() { Id = 0 });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ArtDto.Id);
        }
    }
}