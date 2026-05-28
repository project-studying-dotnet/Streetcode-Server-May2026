using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Validators.Partners;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners
{
    public class StreetcodeShortDtoValidatorTests
    {
        private readonly StreetcodeShortDtoValidator _validator;

        public StreetcodeShortDtoValidatorTests()
        {
            _validator = new StreetcodeShortDtoValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var dto = CreateValidDto();
            dto.Id = invalidId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Title_Is_Empty(string? invalidTitle)
        {
            var dto = CreateValidDto();
            dto.Title = invalidTitle!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Title = new string('A', 256);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_DTO_Is_Fully_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Title_Is_Exactly_At_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Title = new string('A', 255);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static StreetcodeShortDTO CreateValidDto()
        {
            return new StreetcodeShortDTO
            {
                Id = 42,
                Title = "Валідний та короткий заголовок Стріткоду"
            };
        }
    }
}