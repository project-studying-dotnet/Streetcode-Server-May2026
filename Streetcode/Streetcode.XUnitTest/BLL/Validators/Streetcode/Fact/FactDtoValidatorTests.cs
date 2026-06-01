using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.Fact;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Streetcode.Fact
{
    public class FactDtoValidatorTests
    {
        private const int MaxTitleLength = 68;
        private const int MaxFactContentLength = 800;
        private readonly FactDtoValidator _validator;

        public FactDtoValidatorTests()
        {
            _validator = new FactDtoValidator();
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

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(ErrorMessages.TitleIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Title = new string('A', 69);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, MaxTitleLength));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_FactContent_Is_Empty(string? invalidContent)
        {
            var dto = CreateValidDto();
            dto.FactContent = invalidContent!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.FactContent)
                  .WithErrorMessage(ErrorMessages.FactContentIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_FactContent_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.FactContent = new string('B', 801);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.FactContent)
                  .WithErrorMessage(string.Format(ErrorMessages.FactContentMustNotExceedCharacters, MaxFactContentLength));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_ImageId_Is_Less_Or_Equal_To_Zero(int invalidImageId)
        {
            var dto = CreateValidDto();
            dto.ImageId = invalidImageId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.ImageId)
                  .WithErrorMessage(ErrorMessages.ImageIdMustBePositive);
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
            dto.Title = new string('A', 68);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_FactContent_Is_Exactly_At_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.FactContent = new string('B', 800);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static FactDto CreateValidDto()
        {
            return new FactDto
            {
                Id = 1,
                Title = "Історичний факт",
                FactContent = "Цей текст містить цікаві відомості про культурну спадщину Стріткоду.",
                ImageId = 24
            };
        }
    }
}