using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.Term;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Term
{
    public class TermDtoValidatorTests
    {
        private const int MaxDescriptionLength = 250;
        private readonly TermDtoValidator _validator;

        public TermDtoValidatorTests()
        {
            _validator = new TermDtoValidator();
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Description_Is_Empty(string? invalidDescription)
        {
            var dto = CreateValidDto();
            dto.Description = invalidDescription!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage(ErrorMessages.DescriptionIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_250_Characters()
        {
            var dto = CreateValidDto();
            dto.Description = new string('a', 251);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength));
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Description_Is_Exactly_250_Characters()
        {
            var dto = CreateValidDto();
            dto.Description = new string('a', 250);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static TermDto CreateValidDto()
        {
            return new TermDto
            {
                Title = "Валідний заголовок",
                Description = "Валідний опис"
            };
        }
    }
}
