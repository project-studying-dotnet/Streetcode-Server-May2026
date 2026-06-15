using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Validators.Media.Art.Create;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.Art
{
    public class ArtCreateDtoValidatorTests
    {
        private readonly ArtCreateDtoValidator _validator;

        public ArtCreateDtoValidatorTests()
        {
            _validator = new ArtCreateDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_ImageId_Is_Zero()
        {
            var model = new ArtCreateDto { ImageId = 0, Title = "Valid Title" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ImageId);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_Max_Length()
        {
            var model = new ArtCreateDto { ImageId = 1, Title = new string('a', 151) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Data_Is_Valid()
        {
            var model = new ArtCreateDto { ImageId = 1, Title = "Valid Title" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
