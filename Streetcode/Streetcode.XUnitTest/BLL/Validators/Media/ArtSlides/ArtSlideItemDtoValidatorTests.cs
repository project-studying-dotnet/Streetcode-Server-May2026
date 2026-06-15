using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Validators.Media.ArtSlides;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides
{
    public class ArtSlideItemDtoValidatorTests
    {
        private readonly ArtSlideItemDtoValidator _validator = new();

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_ArtId_Is_Not_Positive(int invalidId)
        {
            var model = new ArtSlideItemDto { ArtId = invalidId, Index = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ArtId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Index_Is_Negative(int invalidIndex)
        {
            var model = new ArtSlideItemDto { ArtId = 1, Index = invalidIndex };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Index);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Data_Is_Valid()
        {
            var model = new ArtSlideItemDto { ArtId = 1, Index = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}