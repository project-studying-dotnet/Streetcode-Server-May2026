using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.ArtSlide.GetAllByStreetcodeId;
using Streetcode.BLL.Validators.Media.ArtSlides.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides.GetByStreetcodeId
{
    public class GetAllArtSlidesByStreetcodeIdQueryValidatorTests
    {
        private readonly GetAllArtSlidesByStreetcodeIdQueryValidator _validator = new();

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_StreetcodeId_Is_Not_Positive(int invalidId)
        {
            var query = new GetAllArtSlidesByStreetcodeIdQuery(invalidId);
            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetAllArtSlidesByStreetcodeIdQuery(1);
            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}