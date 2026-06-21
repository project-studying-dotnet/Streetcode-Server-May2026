using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.BLL.Validators.Media.Art;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.Art
{
    public class CreateArtCommandValidatorTests
    {
        private readonly CreateArtCommandValidator _validator;

        public CreateArtCommandValidatorTests()
        {
            _validator = new CreateArtCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_ArtDto_Is_Null()
        {
            var command = new CreateArtCommand(null!);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ArtDto);
        }
    }
}
